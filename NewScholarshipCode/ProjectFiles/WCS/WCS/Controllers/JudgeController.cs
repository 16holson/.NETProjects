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
    [Authorize(Roles = "Judge,Administrator")]
    public class JudgeController : Controller
    {
        private readonly WcsContext _context;
        private readonly UserManager<User> _userManager;

        public JudgeController(WcsContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(JudgeByStudent));
        }

        /// <summary>
        /// Judge all student applications grouped by student
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> JudgeByStudent()
        {
            User currentJudge = await _userManager.GetUserAsync(User);
            AwardCycle currentCycle = Assistant.GetCurrentAwardCycle(_context);
            int currentAwardCycleId = currentCycle.Id;
            List<StudentForm> currentStudentForms = _context.StudentForms
                .Include(s => s.StudentProfile)
                .Include(s => s.Form)
                .Include(s => s.FormRatings)
                .Where(f => f.AwardCycleId == currentAwardCycleId).ToList();

            if (currentStudentForms == null || currentStudentForms.Count == 0)
            {
                return RedirectToAction(nameof(NoApplications));
            }
            List<ScholarshipAward> allAwards = await _context.ScholarshipAwards.Include(a => a.Scholarship).Include(a => a.AwardMonies).ToListAsync();
            List<StudentRating> allStudentRatings = await _context.StudentRatings
                .Where(r => r.JudgeId == currentJudge.Id && r.AwardCycleId == currentAwardCycleId).ToListAsync();
            List<StudentCourse> allStudentCourses = await _context.StudentCourses.Include(c => c.Course).ToListAsync();
            List<StudentPack> studentPacks = new List<StudentPack>();
            List<int> packed = new List<int>();
            foreach (StudentForm studentForm in currentStudentForms)
            {
                if (!packed.Contains(studentForm.StudentProfileId))
                {
                    studentForm.StudentProfile.ScholarshipAwards = allAwards.Where(a => a.StudentProfileId == studentForm.StudentProfileId).ToList();
                    studentForm.StudentProfile.StudentCourses =
                        from c in allStudentCourses
                        where c.StudentId == studentForm.StudentProfileId
                        select c.Course.CourseNumber;
                    packed.Add(studentForm.StudentProfileId);
                    studentPacks.Add(new StudentPack(studentForm.StudentProfile,
                        allStudentRatings.FirstOrDefault(r => r.StudentProfileId == studentForm.StudentProfileId),
                        currentStudentForms.Where(f => f.StudentProfileId == studentForm.StudentProfileId).ToList(),
                        currentJudge.Id, _context));
                }
            }

            studentPacks.Sort();

            JudgeByStudentViewModel model = new JudgeByStudentViewModel
            {
                AwardCycle = currentCycle,
                StudentPacks = studentPacks
            };

            return View(model);
        }

        /// <summary>
        /// Judge all students for one form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> JudgeByForm(int? id)
        {

            User currentJudge = await _userManager.GetUserAsync(User);

            AwardCycle currentCycle = Assistant.GetCurrentAwardCycle(_context);
            int currentAwardCycleId = currentCycle.Id;

            JudgeByFormViewModel model = new JudgeByFormViewModel() {
                AwardCycle = currentCycle,
                FormPacks = new List<FormPack>()
            };

            List<StudentRating> allStudentRatings = await _context.StudentRatings
                .Where(r => r.JudgeId == currentJudge.Id && r.AwardCycleId == currentAwardCycleId).ToListAsync();

            List<StudentCourse> allStudentCourses = await _context.StudentCourses.Include(c => c.Course).ToListAsync();

            List<StudentForm> currentStudentForms = _context.StudentForms
                .Include(s => s.StudentProfile)
                .Include(s => s.Form)
                .Include(s => s.FormRatings)
                .Where(f => f.AwardCycleId == currentAwardCycleId).ToList();

            //Form filter
            if (currentStudentForms == null || currentStudentForms.Count == 0)
            {
                return RedirectToAction(nameof(NoApplications));
            }

            model.SelectedFormId = id ?? currentStudentForms[0].FormId;

            foreach (Form form in _context.Forms.OrderBy(f => f.Title).Include(f => f.FormCriteria).Where(f => f.Active))
            {
                List<StudentForm> applications = currentStudentForms.Where(f => f.FormId == form.Id).ToList();

                FormPack newPack = new FormPack()
                {
                    Form = form,
                    AppliedCount = applications.Count,
                    RatedCount = 0
                };

                //count ratings
                foreach (StudentForm app in applications)
                {
                    if (app.FormRatings.Count(r => r.JudgeId == currentJudge.Id) >= form.FormCriteria.Count &&
                            allStudentRatings.Any(r => r.StudentProfileId == app.StudentProfileId))
                        newPack.RatedCount++;
                }

                if (form.Id == model.SelectedFormId)
                    model.SelectedForm = newPack;
                else
                    model.FormPacks.Add(newPack);
            }

            //Filter applications by form
            currentStudentForms = currentStudentForms.Where(f => f.FormId == model.SelectedFormId).ToList();
            List<ScholarshipAward> allAwards = await _context.ScholarshipAwards.Include(a => a.Scholarship).Include(a => a.AwardMonies).ToListAsync();
            List<StudentPack> studentPacks = new List<StudentPack>();
            List<int> packed = new List<int>();
            foreach (StudentForm studentForm in currentStudentForms)
            {
                if (!packed.Contains(studentForm.StudentProfileId))
                {
                    studentForm.StudentProfile.ScholarshipAwards = allAwards.Where(a => a.StudentProfileId == studentForm.StudentProfileId).ToList();
                    studentForm.StudentProfile.StudentCourses =
                        from c in allStudentCourses
                        where c.StudentId == studentForm.StudentProfileId
                        select c.Course.CourseNumber;
                    packed.Add(studentForm.StudentProfileId);
                    studentPacks.Add(new StudentPack(studentForm.StudentProfile,
                        allStudentRatings.FirstOrDefault(r => r.StudentProfileId == studentForm.StudentProfileId),
                        currentStudentForms.Where(f => f.StudentProfileId == studentForm.StudentProfileId).ToList(),
                        currentJudge.Id, _context));
                }
            }

            studentPacks.Sort();
            model.StudentPacks = studentPacks;

            return View(model);
        }

        public IActionResult NoApplications()
        {
            return View();
        }
    }
}