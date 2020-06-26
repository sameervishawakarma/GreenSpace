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
    public class TimeSlotsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IConfiguration _Configuration;

        public TimeSlotsController(ReservationsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        // GET: api/TimeSlots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlot(string Key, long RoomId,DateTime Date)
        {
            List<ReservationModel> ReserList = new List<ReservationModel>();
            List<TimeSlot> SlotList = new List<TimeSlot>();
            List<TimeSlot> slist = new List<TimeSlot>();
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            SlotList = await _context.TimeSlot.ToListAsync();
            ReserList = await _context.ReservationModels.Where(result => result.RoomId == RoomId && result.ReservationDate == Date).ToListAsync();
            
            foreach (var reser in ReserList)
            {
                DateTime starttime = reser.StartTime;
                DateTime endtime = reser.EndTime;
                foreach (var slt in SlotList)
                {
                    if (slt.Name == starttime.ToString("HH:mm") && DateTime.Compare(Convert.ToDateTime(slt.Name),Convert.ToDateTime(endtime.ToString("HH:mm")))<0)
                    {
                        slist.Add(slt);
                        starttime = starttime.AddMinutes(15.0);
                    }
                }
            }
            foreach(var str in slist)
            {
                SlotList.Remove(str);
            }

            return SlotList;
        }

        // GET: api/TimeSlots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeSlot>> GetTimeSlot(int id)
        {
            var timeSlot = await _context.TimeSlot.FindAsync(id);

            if (timeSlot == null)
            {
                return NotFound();
            }

            return timeSlot;
        }

        // PUT: api/TimeSlots/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeSlot(int id, TimeSlot timeSlot)
        {
            if (id != timeSlot.Id)
            {
                return BadRequest();
            }

            _context.Entry(timeSlot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeSlotExists(id))
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

        // POST: api/TimeSlots
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TimeSlot>> PostTimeSlot(TimeSlot timeSlot)
        {
            _context.TimeSlot.Add(timeSlot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimeSlot", new { id = timeSlot.Id }, timeSlot);
        }

        // DELETE: api/TimeSlots/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TimeSlot>> DeleteTimeSlot(int id)
        {
            var timeSlot = await _context.TimeSlot.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            _context.TimeSlot.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return timeSlot;
        }

        private bool TimeSlotExists(int id)
        {
            return _context.TimeSlot.Any(e => e.Id == id);
        }
    }
}
