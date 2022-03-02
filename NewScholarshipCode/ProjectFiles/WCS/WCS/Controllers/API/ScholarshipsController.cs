using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Models;
using Newtonsoft.Json;

namespace WCS.Controllers
{
    [Produces("text/plain")]
    [Route("api/Scholarships")]
    //[Authorize(Roles = "Administrator")]
    public class ScholarshipsController : Controller
    {
        private readonly WcsContext _context;

        public ScholarshipsController(WcsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string GetScholarships()
        {
            return JsonConvert.SerializeObject(new ScholarshipCarrier(_context));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScholarship([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var scholarship = await _context.Scholarships.SingleOrDefaultAsync(m => m.Id == id);

            if (scholarship == null)
            {
                return NotFound();
            }

            return Ok(scholarship);
        }


        [HttpPost]
        public async Task<IActionResult> PostScholarships([FromBody] ScholarshipCarrier carrier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (Scholarship s in carrier.Scholarships)
            {
                AwardCycle curentcycle = Assistant.GetCurrentAwardCycle(_context);

                if (s.Id == 0)
                {
                    _context.Scholarships.Add(s);

                    foreach (ScholarshipFund f in s.ScholarshipFunds)
                    {
                        f.AwardCycleId = curentcycle.Id;
                        f.ScholarshipId = s.Id;

                        _context.ScholarshipFunds.Add(f);
                    }
                }
                else
                {
                    Scholarship contextscholarship = _context.Scholarships.FirstOrDefault(sc => sc.Id == s.Id);
                    contextscholarship.Name = s.Name;
                    foreach (ScholarshipFund f in s.ScholarshipFunds)
                    {
                        f.AwardCycleId = curentcycle.Id;
                        f.ScholarshipId = s.Id;

                        if (f.Id == 0)
                            _context.ScholarshipFunds.Add(f);
                        else if (f.Name == "remove")
                            _context.ScholarshipFunds.Remove(f);
                        else
                            _context.Entry(f).State = EntityState.Modified;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }


        [HttpPatch]
        public async Task<IActionResult> CopyScholarshipFunds()
        {
            try
            {
            AwardCycle currentCycle = Assistant.GetCurrentAwardCycle(_context);
            AwardCycle previousCycle = Assistant.GetPreviousAwardCycle(currentCycle, _context);
            List<ScholarshipFund> currentFunds = _context.ScholarshipFunds.Where(a => a.AwardCycleId == currentCycle.Id).ToList();

            foreach (ScholarshipFund f in _context.ScholarshipFunds.Where(a => a.AwardCycleId == previousCycle.Id && a.Scholarship.Active).ToList())
            {
                if(!currentFunds.Any(a => a.Name == f.Name && a.ScholarshipId == f.ScholarshipId))
                {
                        _context.ScholarshipFunds.Add(
                            new ScholarshipFund()
                            {
                                AwardCycleId = currentCycle.Id,
                                ScholarshipId = f.ScholarshipId,
                                Name = f.Name,
                                Amount = f.Amount
                            });
                }
            }

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }


            return Ok();
        }
    }
}