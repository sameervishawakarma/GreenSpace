using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RightDetailsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;

        public RightDetailsController(ReservationsDbContext context)
        {
            _context = context;
        }

        // GET: api/RightDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RightDetail>>> GetRightDetail()
        {
            return await _context.RightDetail.ToListAsync();
        }

        // GET: api/RightDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RightDetail>> GetRightDetail(int id)
        {
            var rightDetail = await _context.RightDetail.FindAsync(id);

            if (rightDetail == null)
            {
                return NotFound();
            }

            return rightDetail;
        }

        // PUT: api/RightDetails/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRightDetail(int id, RightDetail rightDetail)
        {
            if (id != rightDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(rightDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RightDetailExists(id))
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

        // POST: api/RightDetails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RightDetail>> PostRightDetail(RightDetail rightDetail)
        {
            _context.RightDetail.Add(rightDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRightDetail", new { id = rightDetail.Id }, rightDetail);
        }

        // DELETE: api/RightDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RightDetail>> DeleteRightDetail(int id)
        {
            var rightDetail = await _context.RightDetail.FindAsync(id);
            if (rightDetail == null)
            {
                return NotFound();
            }

            _context.RightDetail.Remove(rightDetail);
            await _context.SaveChangesAsync();

            return rightDetail;
        }

        private bool RightDetailExists(int id)
        {
            return _context.RightDetail.Any(e => e.Id == id);
        }
    }
}
