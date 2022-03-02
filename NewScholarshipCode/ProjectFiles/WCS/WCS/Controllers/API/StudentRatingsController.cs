using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Models;

namespace WCS.Controllers
{
    [Produces("application/json")]
    [Route("api/StudentRatings")]
    public class StudentRatingsController : Controller
    {
        private readonly WcsContext _context;
        private readonly UserManager<User> _userManager;

        public StudentRatingsController(WcsContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/StudentRatings
        [HttpGet]
        public IEnumerable<StudentRating> GetStudentRatings()
        {
            return _context.StudentRatings;
        }

        // GET: api/StudentRatings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentRating([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var studentRating = await _context.StudentRatings.SingleOrDefaultAsync(m => m.Id == id);

            if (studentRating == null)
            {
                return NotFound();
            }

            return Ok(studentRating);
        }

        // PUT: api/StudentRatings/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutStudentRating([FromRoute] int id, [FromBody] StudentRating studentRating)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != studentRating.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(studentRating).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!StudentRatingExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/StudentRatings
        [HttpPost]
        public async Task<IActionResult> PostStudentRating([FromBody] StudentRating studentRating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User currentJudge = await _userManager.GetUserAsync(User);
            int currentAwardCycleId = Assistant.GetCurrentAwardCycle(_context).Id;

            if (studentRating.Id == 0)
            {
                StudentRating original = await _context.StudentRatings.FirstOrDefaultAsync(r => r.StudentProfileId == studentRating.StudentProfileId &&
                    r.JudgeId == currentJudge.Id && r.AwardCycleId == currentAwardCycleId);

                if (original == null)
                {
                    studentRating.AwardCycleId = currentAwardCycleId;
                    studentRating.JudgeId = currentJudge.Id;
                    _context.StudentRatings.Add(studentRating);
                    await _context.SaveChangesAsync();
                    return Ok(studentRating.Id);
                }
                else
                {
                    original.Rating = studentRating.Rating;
                    await _context.SaveChangesAsync();
                    return Ok(original.Id);
                }
            }
            else if (_context.StudentRatings.Any(r => r.Id == studentRating.Id && r.JudgeId == currentJudge.Id && r.AwardCycleId == currentAwardCycleId))
            {
                _context.Entry(studentRating).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(studentRating.Id);
            }
            else
                return BadRequest("Couldn't verify this rating");
        }

        // DELETE: api/StudentRatings/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStudentRating([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var studentRating = await _context.StudentRatings.SingleOrDefaultAsync(m => m.Id == id);
        //    if (studentRating == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.StudentRatings.Remove(studentRating);
        //    await _context.SaveChangesAsync();

        //    return Ok(studentRating);
        //}

        private bool StudentRatingExists(int id)
        {
            return _context.StudentRatings.Any(e => e.Id == id);
        }
    }
}