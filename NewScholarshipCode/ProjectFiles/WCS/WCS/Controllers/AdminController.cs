/*
    We will create an AwardCycle to control open applications times. Archive is when a cycle has ended/closed.
        * ID, CycleName, OpenDate, CloseDate
    Use the Active flag to find the current cycle.
    We will create a Dispatch Timer on startup to check the status of the current cycle and check if we are starting the next cycle
    Dispatcher Job will set the AllowStudentLogin (in program.cs) to True when we are in current Active Award Cycle.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Models;
using WCS.Services;

namespace WCS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly WcsContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;

        [TempData]
        public string StatusMessage { get; set; }

        public AdminController(WcsContext context, IEmailSender emailSender, UserManager<User> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            AdminDashViewModel model = new AdminDashViewModel()
            {
                AwardCycles = await _context.AwardCycles.OrderByDescending(c => c.StartDate).Where(c => !c.Recycled).ToListAsync()
            };

            return View(model);
        }

        #region User Management
        public async Task<IActionResult> UserList()
        {
            List<User> users = await _context.Users.ToListAsync();
            foreach(User user in users)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).First();
            }
            users.Sort();
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("FirstName,MiddleInitial,LastName,Id,Email, EmailConfirmed, Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Update selected fields only (This avoids the DbUpdate Concurrency Exception)
                    User updatedUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                    updatedUser.FirstName = user.FirstName;
                    updatedUser.LastName = user.LastName;
                    updatedUser.MiddleInitial = user.MiddleInitial;
                    updatedUser.Email = user.Email;
                    updatedUser.EmailConfirmed = user.EmailConfirmed;         

                    //Update
                    _context.Update(updatedUser);
                    await _context.SaveChangesAsync();

                    string role = (await _userManager.GetRolesAsync(updatedUser)).First();

                    if (!role.Equals(user.Role))
                    {
                        await _userManager.AddToRoleAsync(updatedUser, user.Role);
                        await _userManager.RemoveFromRoleAsync(updatedUser, role);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserList));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserList));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        #endregion

        #region Course Management
        public async Task<IActionResult> CourseList()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Courses/Create
        public IActionResult CreateCourse()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse([Bind("Id,CourseName,CourseNumber,ConcurrentCourse")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CourseList));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> EditCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, [Bind("Id,CourseName,CourseNumber,ConcurrentCourse")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CourseList));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> DeleteCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            var course = await _context.Courses.SingleOrDefaultAsync(m => m.Id == id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CourseList));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
        #endregion

        #region Award Cycle Management

        // GET: AwardCycles/Create
        public IActionResult CreateAwardCycle()
        {
            return View();
        }

        // POST: AwardCycles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAwardCycle([Bind("Id,CycleName,StartDate,EndDate,Status")] AwardCycle awardCycle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(awardCycle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(awardCycle);
        }

        // GET: AwardCycles/Edit/5
        public async Task<IActionResult> EditAwardCycle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var awardCycle = await _context.AwardCycles.SingleOrDefaultAsync(m => m.Id == id);
            if (awardCycle == null)
            {
                return NotFound();
            }
            return View(awardCycle);
        }

        // POST: AwardCycles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAwardCycle(int id, [Bind("Id,CycleName,StartDate,EndDate,Status")] AwardCycle awardCycle)
        {
            if (id != awardCycle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(awardCycle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AwardCycleExists(awardCycle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(awardCycle);
        }

        // GET: AwardCycles/Delete/5
        public async Task<IActionResult> DeleteAwardCycle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var awardCycle = await _context.AwardCycles.SingleOrDefaultAsync(m => m.Id == id);
            User currentUser = await _userManager.GetUserAsync(User);

            if (awardCycle == null)
                return NotFound();

            await awardCycle.RecycleAsync(_context, currentUser);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Close the current Award Cycle
        public async Task<IActionResult> CloseAwardCycle()
        {
            AwardCycle cycle = Assistant.GetCurrentAwardCycle(_context);

            if (cycle == null)
                return NotFound("There is no such Award Cycle");

            cycle.Status = CycleStatus.Closed;
            await _context.SaveChangesAsync();

            return RedirectToAction("Report/" + cycle.Id);
        }

        private bool AwardCycleExists(int id)
        {
            return _context.AwardCycles.Any(e => e.Id == id);
        }
        #endregion

        #region Scholarship Form Management

        /***** FORM ADMIN *****/
        public async Task<IActionResult> Forms()
        {
            return View(await _context.Forms
                .Where(f => !f.Recycled)
                .OrderBy(f => f.Title)
                .Include(f => f.FormRequirements)
                .Include(f => f.FormCriteria)
                .Include(f => f.FormFields)
                .Include(f => f.StudentForms)
                .ToListAsync());
        }

        public async Task<IActionResult> FormToggleActive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Form thing = await _context.Forms.FirstOrDefaultAsync(f => f.Id == id);
            thing.Active = !thing.Active;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Forms));
        }

        public async Task<IActionResult> DeleteForm(int? id)
        {
            Form thing = await _context.Forms.FirstOrDefaultAsync(f => f.Id == id);
            User currentUser = await _userManager.GetUserAsync(User);

            if (thing == null)
                return NotFound();

            try
            {
                await thing.RecycleAsync(_context, currentUser);
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction(nameof(Forms));
        }

        #endregion

        #region Scholarship Management

        public IActionResult Scholarships()
        {
            return View(_context.Scholarships.OrderByDescending(s => s.Active).ThenBy(s => s.Name).Include(s => s.ScholarshipAwards).ToList());
        }

        public async Task<IActionResult> ScholarshipToggleActive(int? id)
        {
            if (id != null)
            {
                Scholarship ship = _context.Scholarships.FirstOrDefault(s => s.Id == id);

                if (ship != null)
                {
                    ship.Active = !ship.Active;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Scholarships));
        }

        #endregion

        #region Invite Management
        // GET : Invites
        public async Task<IActionResult> Invites()
        {
            return View(await _context.Invites.ToListAsync());
        }

        // GET: Invites/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Invites/Create
        public IActionResult CreateInvite()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvite([Bind("Id,FirstName,LastName,EmailAddress,Role")] Invite invite)
        {
            if (ModelState.IsValid)
            {
                invite.ExpirationDate = DateTime.Now.AddDays(14);
                invite.Status = "Invited";
                invite.GenerateInviteCode();
                _context.Add(invite);
                await _context.SaveChangesAsync();

                //Send invite email
                var callbackUrl = Url.InviteCallbackLink(invite.InviteCode, Request.Scheme);
                await _emailSender.SendInviteAsync(invite.EmailAddress, invite.FullName, invite.Role, callbackUrl);

                return RedirectToAction(nameof(Invites));
            }
            return View(invite);
        }

        public async Task<IActionResult> EditInvite(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites.SingleOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }
            return View(invite);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInvite(int id, [Bind("Id,FirstName,LastName,EmailAddress,Role")] Invite invite)
        {
            if (id != invite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    invite.ExpirationDate = DateTime.Now.AddDays(14);
                    invite.Status = "Invited";
                    invite.GenerateInviteCode();

                    _context.Update(invite);
                    await _context.SaveChangesAsync();

                    //Send invite email
                    var callbackUrl = Url.InviteCallbackLink(invite.InviteCode, Request.Scheme);
                    await _emailSender.SendInviteAsync(invite.EmailAddress, invite.FullName, invite.Role, callbackUrl);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InviteExists(invite.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Invites));
            }
            return View(invite);
        }

        // GET: Invites/Delete/5
        public async Task<IActionResult> DeleteInvite(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .SingleOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // POST: Invites/Delete/5
        [HttpPost, ActionName("DeleteInvite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInviteConfirmed(int id)
        {
            var invite = await _context.Invites.SingleOrDefaultAsync(m => m.Id == id);
            _context.Invites.Remove(invite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteExists(int id)
        {
            return _context.Invites.Any(e => e.Id == id);
        }
        #endregion

        #region Settings
        public async Task<IActionResult> EditEmailSettings()
        {
            EmailSettings model = Assistant.GetEmailSettings(_context);
            model.Password = "";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmailSettings(EmailSettings setting)
        {
            if (ModelState.IsValid)
            {
                Assistant.SaveEMailSettings(setting, _context);

                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        #endregion

        #region RecycleBin

        public async Task<IActionResult> RecycleBin()
        {
            RecycleBinViewModel model = new RecycleBinViewModel()
            {
                StatusMessage = StatusMessage,
                RecycledItems = await _context.RecycleBin.OrderByDescending(i => i.RecycledDate).ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
                return NotFound();

            RecycledItem item = await _context.RecycleBin.FirstOrDefaultAsync(i => i.Id == id);

            await item.RestoreItemAsync(_context);

            StatusMessage = $"Restored { item.ItemType }: { item.ItemName }";

            return RedirectToAction(nameof(RecycleBin));
        }

        #endregion

        #region Editable Pages
        public IActionResult EditHomePage()
        {
            var frontPageSetting = _context.Settings.FirstOrDefault(s => s.SettingName.Equals("Front Page Message"));
            var appPageSetting = _context.Settings.FirstOrDefault(s => s.SettingName.Equals("HelpPageMessage"));

            return View(new EditHomePageViewModel()
            {
                ApplicationPageSetting = appPageSetting,
                HomePageSetting = frontPageSetting
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHomePage(string editordata)
        {
            return View("Index");
        }
        #endregion

        #region Reports

        public async Task<IActionResult> Report(int? id)
        {
            ReportViewModel model = null;

            if (id == null)
                return View(model);

            AwardCycle cycle = _context.AwardCycles.FirstOrDefault(a => a.Id == id);

            if (cycle != null)
            {
                model = new ReportViewModel()
                {
                    AwardCycle = cycle,
                    Scholarships = (from s in _context.Scholarships.Include(s => s.ScholarshipAwards)
                                    where s.ScholarshipAwards.Any(a => a.AwardCycleId == cycle.Id)
                                    select s).ToList()
                };

                List<ScholarshipAward> awards = await _context.ScholarshipAwards
                    .OrderBy(a => a.Scholarship.Name)
                    .Include(a => a.StudentProfile)
                    .Where(a => a.AwardCycleId == cycle.Id)
                    .ToListAsync();

                List<ScholarshipAwardMoney> monies = _context.ScholarshipAwardMonies
                    .Include(m => m.ScholarshipFund)
                    .Where(m => m.ScholarshipAward.AwardCycleId == cycle.Id).ToList();

                foreach (ScholarshipAward award in awards)
                {
                    award.AwardMonies = monies.Where(m => m.ScholarshipAwardId == award.Id).ToList();
                }

                foreach (Scholarship ship in model.Scholarships)
                {
                    ship.ScholarshipAwards = awards.Where(a => a.ScholarshipId == ship.Id).ToList();
                }
            }

            return View(model);
        }

        #endregion
    }
}