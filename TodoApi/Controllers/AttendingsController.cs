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
    public class AttendingsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IConfiguration _Configuration;

        public AttendingsController(ReservationsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        // GET: api/Attendings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attending>>> GetAttendings(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.Attendings.ToListAsync();
        }

        // GET: api/Attendings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Attending>> GetAttending(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var attending = await _context.Attendings.FindAsync(id);

            if (attending == null)
            {
                return NotFound();
            }

            return attending;
        }

        // PUT: api/Attendings/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttending(long id, Attending attending)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(attending.Key, _Configuration);
            if (id != attending.Id)
            {
                return BadRequest();
            }

            _context.Entry(attending).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendingExists(id, attending.Key))
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

        // POST: api/Attendings
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Attending>> PostAttending(Attending attending)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(attending.Key, _Configuration);
            _context.Attendings.Add(attending);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttending", new { id = attending.Id, Key = attending.Key }, attending);
        }

        // DELETE: api/Attendings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Attending>> DeleteAttending(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var attending = await _context.Attendings.FindAsync(id);
            if (attending == null)
            {
                return NotFound();
            }

            _context.Attendings.Remove(attending);
            await _context.SaveChangesAsync();

            return attending;
        }

        private bool AttendingExists(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.Attendings.Any(e => e.Id == id);
        }
    }
}
