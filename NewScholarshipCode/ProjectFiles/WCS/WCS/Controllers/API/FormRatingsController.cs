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
    [Produces("application/json")]
    [Route("api/FormRatings")]
    [Authorize(Roles = "Judge, Administrator")]
    public class FormRatingsController : Controller
    {
        private readonly WcsContext _context;
        private readonly UserManager<User> _userManager;

        public FormRatingsController(WcsContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> PostFormRating([FromBody] FormRating formRating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User currentJudge = await _userManager.GetUserAsync(User);

            if (formRating.Id == 0)
            {
                formRating.JudgeId = currentJudge.Id;
                if (!_context.FormRatings
                    .Any(r => r.JudgeId == currentJudge.Id && r.FormCriterionId == formRating.FormCriterionId && r.StudentFormId == formRating.StudentFormId))
                    _context.FormRatings.Add(formRating);
                else
                {
                    FormRating editedFormRating = _context.FormRatings
                        .FirstOrDefault(r => r.JudgeId == currentJudge.Id && r.FormCriterionId == formRating.FormCriterionId && r.StudentFormId == formRating.StudentFormId);

                    if (editedFormRating != null)
                    {
                        editedFormRating.Rating = formRating.Rating;
                        _context.Entry(editedFormRating).State = EntityState.Modified;
                    }
                    else return BadRequest("Error finding rating item");
                }
            }
            else
            {
                if (!_context.FormRatings.Any(r => r.Equals(formRating)))
                    return BadRequest("Error matching item");

                formRating.JudgeId = currentJudge.Id;
                _context.Entry(formRating).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FormRatingExists(formRating.Id))
                {
                    return BadRequest("Couldn't find this rating");
                }
                else
                {
                    return BadRequest(ex);
                }
            }

            return Ok();
        }

        // GET: api/FormRatings
        //[HttpGet]
        //public IEnumerable<FormRating> GetFormRatings()
        //{
        //    return _context.FormRatings;
        //}

        // GET: api/FormRatings/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetFormRating([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var formRating = await _context.FormRatings.SingleOrDefaultAsync(m => m.Id == id);

        //    if (formRating == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(formRating);
        //}

        private bool FormRatingExists(int id)
        {
            return _context.FormRatings.Any(e => e.Id == id);
        }
    }
}