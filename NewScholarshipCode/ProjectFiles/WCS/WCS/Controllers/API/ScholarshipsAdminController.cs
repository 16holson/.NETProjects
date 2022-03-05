using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace WCS.Controllers.API
{
    [Produces("text/plain")]
    [Route("api/ScholarshipsAdmin")]
    [Authorize(Roles = "Administrator")]
    public class ScholarshipsAdminController : Controller
    {
        private readonly WcsContext _context;

        public ScholarshipsAdminController(WcsContext context)
        {
            _context = context;
        }

        // GET: api/ScholarshipsAdmin
        [HttpGet]
        public IActionResult GetEditorPack()
        {
            ScholarshipAwardEditorPack pack = new ScholarshipAwardEditorPack(_context);

            return Ok(JsonConvert.SerializeObject(pack));
        }

        // GET: api/ScholarshipsAdmin/5
        [HttpGet("{id}")]
        public IActionResult GetStudentAward([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        // POST: api/ScholarshipsAdmin
        [HttpPost]
        public async Task<IActionResult> PostScholarshipAward([FromBody] ScholarshipAward award)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int currentCycleId = Assistant.GetCurrentAwardCycle(_context).Id;

            //Add a new award
            ScholarshipAward theAward;
            if (award.Id == 0)
            {
                theAward = new ScholarshipAward()
                {
                    StudentProfileId = award.StudentProfileId,
                    ScholarshipId = award.ScholarshipId,
                    AwardCycleId = currentCycleId
                };

                _context.ScholarshipAwards.Add(theAward);
                _context.SaveChanges();

                //Add funds > 0
                foreach (ScholarshipAwardMoney money in award.AwardMonies)
                {
                    if (money.Amount > 0)
                    {
                        money.ScholarshipAwardId = theAward.Id;
                        _context.ScholarshipAwardMonies.Add(money);
                    }
                }

                await _context.SaveChangesAsync();
            }
            //Update an award
            else
            {
                theAward = await _context.ScholarshipAwards
                    .Include(a => a.Scholarship)
                    .Include(a => a.AwardMonies)
                    .FirstOrDefaultAsync(a => a.Id == award.Id);

                if (theAward == null)
                    return NotFound("ScholarshipAward Not Found");

                theAward.StudentProfileId = award.StudentProfileId;
                theAward.ScholarshipId = award.ScholarshipId;

                //Delete previous unmatching funds if necessary
                foreach (ScholarshipAwardMoney amoney in theAward.AwardMonies)
                {
                    if (!award.AwardMonies.Any(m => m.ScholarshipAwardId == amoney.ScholarshipAwardId && m.ScholarshipFundId == amoney.ScholarshipFundId))
                    {
                        _context.Remove(amoney);
                        continue;
                    }

                    //Remove monies tied to a different award
                    _context.RemoveRange(_context.ScholarshipAwardMonies.Where(m => m.ScholarshipAwardId == theAward.Id && m.ScholarshipFund.ScholarshipId != theAward.ScholarshipId));
                }

                await _context.SaveChangesAsync();

                //Add or Update monies
                foreach (ScholarshipAwardMoney money in award.AwardMonies)
                {
                    if (money.Id == 0)
                    {
                        ScholarshipAwardMoney cMoney = _context.ScholarshipAwardMonies
                            .FirstOrDefault(m => m.ScholarshipAwardId == money.ScholarshipAwardId && m.ScholarshipFundId == money.ScholarshipFundId && m.ScholarshipFund.AwardCycleId == currentCycleId);

                        if (cMoney == null && money.Amount > 0)
                        {
                            money.ScholarshipAwardId = theAward.Id;
                            _context.ScholarshipAwardMonies.Add(money);
                        }
                        else if (cMoney != null)
                        {
                            if (money.Amount > 0)
                                cMoney.Amount = money.Amount;
                            else
                                _context.Remove(cMoney);
                        }
                    }
                    else
                    {
                        ScholarshipAwardMoney cMoney = _context.ScholarshipAwardMonies.FirstOrDefault(m => m.Id == money.Id);

                        if (cMoney == null)
                            continue;
                        cMoney.Amount = money.Amount;

                        if (cMoney.Amount == 0)
                            _context.Remove(cMoney);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}