using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Models;

namespace WCS.Controllers
{

    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly WcsContext _context;
        private readonly UserManager<User> _userManager;

        public StudentController(WcsContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (user.StudentProfileId == null)
                {
                    return RedirectToAction(nameof(CreateStudentProfile));
                }

                //Get all the forms (and responses)
                List<StudentFormPack> model = new List<StudentFormPack>();

                AwardCycle currentCycle = Assistant.GetCurrentApplicationCycle(_context);

                List<StudentForm> studentForms = _context.StudentForms
                    .Include(f => f.StudentResponses)
                    .Where(f => f.StudentProfileId == user.StudentProfileId && f.AwardCycleId == currentCycle.Id)
                    .ToList();

                foreach (Form form in _context.Forms.Include(f => f.FormRequirements).OrderBy(f => f.Title).Where(f => f.Active).ToList())
                {
                    if (StudentCanViewForm(form, _context.StudentProfiles.First(p => p.Id == user.StudentProfileId)))
                        model.Add(new StudentFormPack(
                            form,
                            studentForms.FirstOrDefault(f => f.FormId == form.Id),
                            _context
                            ));
                }

                model.Sort();
                ViewBag.HelpPageMessage = Assistant.GetSettings(SettingName.HelpPageMessage, _context);
                return View(model);
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        #region Student Profile Functions

        // GET: StudentProfiles/Create
        public async Task<IActionResult> CreateStudentProfile()
        {
            //Get User
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                //If the user has a profile redirect them to the edit page.
                if (user.StudentProfileId != null) return RedirectToAction("EditStudentProfile");

                //Create a new profile
                StudentProfile model = new StudentProfile();
                model.BirthDate = DateTime.Now.AddYears(-18);
                List<string> intlist = new List<string>();
                //Fill with available courses
                model.FillAvailableCourses(_context);

                return View(model);
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        // POST: StudentProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudentProfile([Bind("Prefix,BirthDate,Address,Address2,City,State,ZipCode,Gender," +
            "MaritalStatus,OverallGPA,MajorGPA,ActScore,CurrentMajor,FutureMajor,CurrentAcademicLevel,UltimateDegreeGoal,HighSchool," +
            "LastUniversity,FirstWsuSemester,FirstWsuYear,CurrentScheduleStatus,ApTestList,ClubAndOrganizationHistory,AwardsHistory," +
            "CsInterest,ScholarshipAidHistory,AchievementsHistory, StudentCourses, WNumber")] StudentProfile profile)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction(nameof(Index), "Home");
                profile.FirstName = user.FirstName;
                profile.LastName = user.LastName;
                _context.Add(profile);
                var result = await _context.SaveChangesAsync();

                User me = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                me.StudentProfileId = profile.Id;

                profile.PastCourses = new List<StudentCourse>();

                if (profile.StudentCourses != null)
                {
                    foreach (string thing in profile.StudentCourses)
                    {
                        _context.StudentCourses.Add(new StudentCourse
                        {
                            CourseId = int.Parse(thing),
                            StudentId = profile.Id
                        });
                    }
                }

                if (profile.ApTestList != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string test in profile.ApTestList)
                    {
                        sb.Append(test + ",");
                    }
                    profile.PassedApTests = sb.ToString().Substring(0, sb.ToString().Length - 1);
                }

                var StudentTable = _context.StudentCourses;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        public async Task<IActionResult> EditStudentProfile()
        {
            //Get User
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Index), "Home");
            //If not profile then redirect them to create one
            if (user.StudentProfileId == null) return RedirectToAction("CreateStudentProfile");

            //Pull in the current profile for the user.
            StudentProfile model = _context.StudentProfiles.FirstOrDefault(p => p.Id == user.StudentProfileId);
            //Fill the available course in the model
            model.FillAvailableCourses(_context);
            //Fill ApTest
            if (model.PassedApTests != null) model.ApTestList = model.PassedApTests.Split(',');

            return View(model);
        }

        // POST: StudentProfiles/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentProfile([Bind("Id,Prefix,BirthDate,Address,Address2,City,State,ZipCode,Gender," +
            "MaritalStatus,OverallGPA,MajorGPA,ActScore,CurrentMajor,FutureMajor,CurrentAcademicLevel,UltimateDegreeGoal,HighSchool," +
            "LastUniversity,FirstWsuSemester,FirstWsuYear,CurrentScheduleStatus,ApTestList,ClubAndOrganizationHistory,AwardsHistory," +
            "CsInterest,ScholarshipAidHistory,AchievementsHistory, StudentCourses, WNumber")] StudentProfile profile)
        {
            if (ModelState.IsValid)
            {
                //Grab the user
                User user = await _userManager.GetUserAsync(User);

                //Update profile
                _context.Update(profile);
                profile.FirstName = user.FirstName;
                profile.LastName = user.LastName;
                var result = await _context.SaveChangesAsync();

                //Remove courses for user
                _context.StudentCourses.RemoveRange(_context.StudentCourses.Where(c => c.StudentId == profile.Id));
                result = await _context.SaveChangesAsync();

                //Add Courses
                profile.PastCourses = new List<StudentCourse>();
                if (profile.StudentCourses != null)
                {
                    foreach (string thing in profile.StudentCourses)
                    {
                        _context.StudentCourses.Add(new StudentCourse
                        {
                            CourseId = int.Parse(thing),
                            StudentId = profile.Id
                        });
                    }
                }

                if (profile.ApTestList != null)
                {
                    if (profile.ApTestList != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (string test in profile.ApTestList)
                        {
                            sb.Append(test + ",");
                        }
                        profile.PassedApTests = sb.ToString().Substring(0, sb.ToString().Length - 1);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Pass a form and student profile, and return true if the student meets all requirements
        /// </summary>
        /// <param name="form"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        public bool StudentCanViewForm(Form form, StudentProfile student)
        {
            try
            {
                if (form.FormRequirements == null)
                    _context.Entry(form).Collection(f => f.FormRequirements).Load();

                foreach (FormRequirement req in form.FormRequirements)
                {
                    if (Assistant.FormRequirementMap[req.ProfileField].Equals("Text"))
                    {
                        string studentval = student.GetType().GetProperty(req.ProfileField.Replace(" ", "")).GetValue(student, null) as string;

                        switch (req.Operator)
                        {
                            case "Equals":
                                if (!studentval.Equals(req.Value))
                                    return false;
                                break;
                            case "Not Equals":
                                if (studentval.Equals(req.Value))
                                    return false;
                                break;
                            case "Contains":
                                if (!studentval.Contains(req.Value))
                                    return false;
                                break;
                            case "Not Contains":
                                if (studentval.Contains(req.Value))
                                    return false;
                                break;
                        }
                        Console.WriteLine(req.ProfileField + " of " + studentval + " is " + req.Operator + " " + req.Value);
                    }
                    else
                    {
                        Double studentval;
                        if (!Double.TryParse((student.GetType().GetProperty(req.ProfileField.Replace(" ", "")).GetValue(student, null).ToString()), out studentval))
                            studentval = 0.0;

                        Double reqval;
                        if (!Double.TryParse(req.Value, out reqval))
                            reqval = 0.0;

                        switch (req.Operator)
                        {
                            case "Equals":
                                if (studentval != reqval)
                                    return false;
                                break;
                            case "Greater Than":
                                if (studentval <= reqval)
                                    return false;
                                break;
                            case "Greater Than or Equal to":
                                if (studentval < reqval)
                                    return false;
                                break;
                            case "Less Than":
                                if (studentval >= reqval)
                                    return false;
                                break;
                            case "Less Than or Equal to":
                                if (studentval > reqval)
                                    return false;
                                break;
                        }
                        Console.WriteLine(req.ProfileField + " of " + studentval + " is " + req.Operator + " " + req.Value);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}