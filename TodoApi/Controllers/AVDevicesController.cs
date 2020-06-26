using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TodoApi.DTO;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AVDevicesController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IConfiguration _Configuration;

        public AVDevicesController(ReservationsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        // GET: api/AVDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AVDevices>>> GetAVDevices(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.AVDevices.ToListAsync();
        }

        // GET: api/AVDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AVDevices>> GetAVDevices(int id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var aVDevices = await _context.AVDevices.FindAsync(id);

            if (aVDevices == null)
            {
                return NotFound();
            }

            return aVDevices;
        }

        // PUT: api/AVDevices/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAVDevices(int id, AVDevices aVDevices)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(aVDevices.Key, _Configuration);
            if (id != aVDevices.Id)
            {
                return BadRequest();
            }

            _context.Entry(aVDevices).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AVDevicesExists(id, aVDevices.Key))
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

        // POST: api/AVDevices
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<AVDevices>> PostAVDevices(AVDevices aVDevices)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(aVDevices.Key, _Configuration);
            _context.AVDevices.Add(aVDevices);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAVDevices", new { id = aVDevices.Id }, aVDevices);
        }

        // DELETE: api/AVDevices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AVDevices>> DeleteAVDevices(int id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var aVDevices = await _context.AVDevices.FindAsync(id);
            if (aVDevices == null)
            {
                return NotFound();
            }

            _context.AVDevices.Remove(aVDevices);
            await _context.SaveChangesAsync();

            return aVDevices;
        }

        private bool AVDevicesExists(int id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.AVDevices.Any(e => e.Id == id);
        }
    }
}
