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
    [Produces("text/plain")]
    [Route("api/StudentForms")]
    public class StudentFormsController : Controller
    {
        private readonly WcsContext _context;
        private readonly UserManager<User> _userManager;

        public StudentFormsController(WcsContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/StudentForms
        [HttpGet]
        public IEnumerable<StudentForm> GetStudentForms()
        {
            return _context.StudentForms;
        }

        // GET: api/StudentForms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentForm([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var studentForm = await _context.StudentForms.SingleOrDefaultAsync(m => m.Id == id);

            if (studentForm == null)
            {
                return NotFound();
            }

            return Ok(studentForm);
        }

        // POST: api/StudentForms
        [HttpPost]
        public async Task<IActionResult> PostStudentForm([FromBody] StudentForm studentForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User currentUser = await _userManager.GetUserAsync(User);
            int currentAppCycleId = Assistant.GetCurrentApplicationCycle(_context).Id;

            if (studentForm.Id == 0)
            {
                if (_context.StudentForms.Any(f => f.FormId == studentForm.FormId && f.StudentProfileId == currentUser.StudentProfileId && f.AwardCycleId == currentAppCycleId))
                    return BadRequest("Invalid Student Form");

                studentForm.StudentProfileId = currentUser.StudentProfileId ?? 0;
                studentForm.AwardCycleId = currentAppCycleId;
                _context.StudentForms.Add(studentForm);
                await _context.SaveChangesAsync();
            }
            else if (studentForm.StudentProfileId != currentUser.StudentProfileId ||
              studentForm.AwardCycleId != currentAppCycleId)
            {
                return BadRequest("Invalid Student Form Ownership");
            }

            foreach (FormResponse response in studentForm.StudentResponses)
            {
                //Verify formfield is part of the same form as the studentform
                var field = await _context.FormFields.FirstOrDefaultAsync(f => f.Id == response.FormFieldId);
                if (field.FormId != studentForm.FormId)
                {
                    return BadRequest("Invalid Form-Field Match");
                }
                if (response.Id == 0)
                {
                    response.StudentFormId = studentForm.Id;

                    if (_context.FormResponses.Any(r => r.FormFieldId == response.FormFieldId && r.StudentFormId == response.StudentFormId))
                        return BadRequest("Invalid Response Received");

                    _context.FormResponses.Add(response);
                }
                else
                {
                    _context.Entry(response).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        //// DELETE: api/StudentForms/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStudentForm([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var studentForm = await _context.StudentForms.SingleOrDefaultAsync(m => m.Id == id);
        //    if (studentForm == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.StudentForms.Remove(studentForm);
        //    await _context.SaveChangesAsync();

        //    return Ok(studentForm);
        //}

        private bool StudentFormExists(int id)
        {
            return _context.StudentForms.Any(e => e.Id == id);
        }
    }
}