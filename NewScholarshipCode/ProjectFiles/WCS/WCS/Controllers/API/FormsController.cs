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
using Microsoft.AspNetCore.Identity;

namespace WCS.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Produces("text/plain")]
    [Route("api/Forms")]
    public class FormsController : Controller
    {
        private readonly WcsContext _context;
        private readonly UserManager<User> _userManager;

        public FormsController(WcsContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Forms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForm([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Form form;

            try
            {
                form = await _context.Forms
                    .Include(m => m.FormCriteria)
                    .Include(m => m.FormRequirements)
                    .Include(m => m.FormFields)
                    .SingleOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (form == null)
            {
                return NotFound();
            }

            //Sort criteria and fields by order
            if (form.FormCriteria != null)
                form.FormCriteria.Sort();
            if (form.FormFields != null)
                form.FormFields.Sort();

            return Ok(JsonConvert.SerializeObject(form));
        }

        // PUT: api/Forms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForm([FromRoute] int id, [FromBody] Form form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != form.Id)
            {
                return BadRequest();
            }

            _context.Entry(form).State = EntityState.Modified;
            User currentUser = await _userManager.GetUserAsync(User);

            //Deal with Requirements
            foreach (FormRequirement requirement in form.FormRequirements)
            {
                if (requirement.Id < 0)
                {
                    requirement.FormId = form.Id;
                    requirement.Id = 0;
                    _context.FormRequirements.Add(requirement);
                }
                else if (requirement.Recycled)
                {
                    await requirement.RecycleAsync(_context, currentUser);
                }
                else
                {
                    _context.Entry(requirement).State = EntityState.Modified;
                }
            }

            //Deal with Criteria
            foreach (FormCriterion criterion in form.FormCriteria)
            {
                if (criterion.Id < 0)
                {
                    criterion.FormId = form.Id;
                    criterion.Id = 0;
                    _context.FormCriteria.Add(criterion);
                }
                else if (criterion.Recycled)
                {
                    await criterion.RecycleAsync(_context, currentUser);
                }
                else
                {
                    _context.Entry(criterion).State = EntityState.Modified;
                }
            }

            //Deal with Form Fields
            foreach (FormField field in form.FormFields)
            {
                if (field.Id < 0)
                {
                    field.FormId = form.Id;
                    field.Id = 0;
                    _context.FormFields.Add(field);
                }
                else if (field.Recycled)
                {
                    await field.RecycleAsync(_context, currentUser);
                }
                else
                {
                    _context.Entry(field).State = EntityState.Modified;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Forms
        [HttpPost]
        public async Task<IActionResult> PostForm([FromBody] Form form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (form.Id == 0)
                {
                    _context.Forms.Add(form);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetForm", new { id = form.Id }, JsonConvert.SerializeObject(form));
                }
                else
                {
                    Form original = _context.Forms
                        .Include(f => f.FormRequirements)
                        .Include(f => f.FormCriteria)
                        .Include(f => f.FormFields)
                        .AsNoTracking()
                        .FirstOrDefault(f => f.Id == form.Id);

                    if (original == null)
                        return BadRequest("Couldn't find that form");

                    Form copy = new Form()
                    {
                        Title = original.Title + " - Copy",
                        Description = original.Description,
                        Active = false,
                        IncludeStudentRating = original.IncludeStudentRating,
                        FormRequirements = new List<FormRequirement>(),
                        FormCriteria = new List<FormCriterion>(),
                        FormFields = new List<FormField>()
                    };

                    foreach (FormRequirement req in original.FormRequirements)
                    {
                        copy.FormRequirements.Add(new FormRequirement()
                        {
                            Operator = req.Operator,
                            ProfileField = req.ProfileField,
                            Value = req.Value
                        });
                    }

                    foreach (FormCriterion cri in original.FormCriteria)
                    {
                        copy.FormCriteria.Add(new FormCriterion()
                        {
                            Title = cri.Title,
                            Order = cri.Order
                        });
                    }

                    foreach (FormField fld in original.FormFields)
                    {
                        copy.FormFields.Add(new FormField()
                        {
                            Type = fld.Type,
                            Title = fld.Title,
                            Order = fld.Order,
                            Required = fld.Required,
                            Options = fld.Options
                        });
                    }

                    _context.Forms.Add(copy);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetForm", new { id = copy.Id }, JsonConvert.SerializeObject(copy));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        private bool FormExists(int id)
        {
            return _context.Forms.Any(e => e.Id == id);
        }
    }
}