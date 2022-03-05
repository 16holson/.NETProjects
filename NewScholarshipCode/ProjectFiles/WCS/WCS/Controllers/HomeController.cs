using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WCS.Data;
using WCS.Models;
using WCS.Models.HomeViewModels;
using WCS.Services;

namespace WCS.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;

        private readonly WcsContext _context;

        [TempData]
        public string StatusTitle { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public HomeController(UserManager<User> userManager, WcsContext context, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            AwardCycle openCycle = Assistant.GetCurrentApplicationCycle(_context);
            AwardCycle nextCycle = Assistant.GetNextAwardCycle(_context);
            DateTime today = DateTime.Now;

            //Check for the next cycle start
            if (openCycle == null && nextCycle != null)
            {
                if (nextCycle.StartDate <= today)
                {
                    nextCycle.Status = CycleStatus.Open;
                    await _context.SaveChangesAsync();
                    openCycle = nextCycle;
                }
            }

            //Check if current cycle has ended
            if (openCycle != null && openCycle.EndDate < today)
            {
                openCycle.Status = CycleStatus.Judging;
                var result = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                //Update login for user
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                //Check if user is an admin
                if (await _userManager.IsInRoleAsync(user, "Administrator"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                //Check if user is a judge
                else if (await _userManager.IsInRoleAsync(user, "Judge"))
                {
                    return RedirectToAction("Index", "Judge");
                }
                //If they have logged in but have no other role assume the are a student
                else
                {
                    if (openCycle == null)
                    {
                        StatusTitle = "Applications are currently closed";
                        StatusMessage = "You cannot login until the next application period opens";
                        return await forceLogOut();
                    }
                    else
                    {
                        return RedirectToAction("Index", "Student");
                    }
                }
            }

            //Custom home page area
            ViewBag.HomePage = Assistant.GetSettings(SettingName.FrontPageMessage, _context);
            if (ViewBag.HomePage == null) ViewBag.HomePage = "";

            MessageViewModel model = new MessageViewModel
            {
                ApplicationStatus = "CLOSED"
            };

            if (openCycle != null)
            {
                model.ApplicationStatus = "OPEN";
                model.ApplicationCloses = openCycle.EndDate.ToLongDateString();
            }
            else if (nextCycle != null)
            {
                model.ApplicationOpens = nextCycle.StartDate.ToShortDateString();
                model.ApplicationCloses = nextCycle.EndDate.ToShortDateString();
            }

            if (StatusMessage != null)
            {
                model.Title = StatusTitle;
                model.Body = StatusMessage;
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MessageViewModel model, string returnUrl = null)
        {
            //Custom home page area
            ViewBag.HomePage = Assistant.GetSettings(SettingName.FrontPageMessage, _context);
            if (ViewBag.HomePage == null) ViewBag.HomePage = "";

            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //TODO Uncomment when we put email verification in place
                //Check for confirmed email
                //var user = await _userManager.FindByEmailAsync(model.Email);
                //if (user != null && !user.EmailConfirmed)
                //{
                //    ModelState.AddModelError(string.Empty, "You must confirm your email to log in.");
                //    return View();
                //}

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult SendVerificationEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new ApplicationException($"Unable to find user with email of '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            StatusTitle = "You've got mail!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<IActionResult> forceLogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
