using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Models;

namespace WCS.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Settings")]
    public class SettingsController : Controller
    {
        private readonly WcsContext _context;

        public SettingsController(WcsContext context)
        {
            _context = context;
        }

        // GET: api/Settings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSetting([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var setting = await _context.Settings.SingleOrDefaultAsync(m => m.Id == id);

            if (setting == null)
            {
                return NotFound();
            }

            return Ok(setting);
        }

        // PUT: api/Settings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSetting([FromRoute] int id, [FromBody] Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != setting.Id)
            {
                return BadRequest();
            }

            _context.Entry(setting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Settings
        [HttpPost]
        public async Task<IActionResult> PostSetting([FromBody] Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                SettingName nameofsetting;

                if (setting.SettingName == "HelpPageMessage")
                    nameofsetting = SettingName.HelpPageMessage;
                else
                    nameofsetting = SettingName.FrontPageMessage;

                Assistant.SaveSetting(nameofsetting, setting.SettingValue, _context);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        //// DELETE: api/Settings/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteSetting([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var setting = await _context.Settings.SingleOrDefaultAsync(m => m.Id == id);
        //    if (setting == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Settings.Remove(setting);
        //    await _context.SaveChangesAsync();

        //    return Ok(setting);
        //}

        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}