using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Models;
using Microsoft.AspNetCore.Authorization;

namespace WCS.Controllers
{
    [Authorize(Roles = "Administrator, Judge")]
    public class ScholarshipAwardsController : Controller
    {
        private readonly WcsContext _context;

        public ScholarshipAwardsController(WcsContext context)
        {
            _context = context;
        }

        // GET: ScholarshipAwards
        public async Task<IActionResult> Index()
        {
            ScholarshipAwardsViewModel model = new ScholarshipAwardsViewModel()
            {
                AwardCycle = Assistant.GetCurrentAwardCycle(_context),
                Scholarships = new ScholarshipCarrier(_context).Scholarships,
                AwardsListModel = new ScholarshipAwardsListModel(_context),
                StudentRatingsListPack = new StudentRatingsListPack(_context)
            };

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAward(int? id)
        {
            int currentCycleId = Assistant.GetCurrentAwardCycle(_context).Id;
            ScholarshipAward award = _context.ScholarshipAwards.Include(a => a.AwardMonies).FirstOrDefault(a => a.Id == id && a.AwardCycleId == currentCycleId);

            if (award == null)
                return NotFound("Award Not Found or Is not in the current Award Cycle");

            foreach (ScholarshipAwardMoney money in award.AwardMonies)
                _context.ScholarshipAwardMonies.Remove(money);

            _context.ScholarshipAwards.Remove(award);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
