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
    public class RolePriviligaesController : ControllerBase
    {
        private readonly ReservationsDbContext _context;

        public RolePriviligaesController(ReservationsDbContext context)
        {
            _context = context;
        }

        // GET: api/RolePriviligaes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolePriviligae>>> GetRolePriviligae(UserRoles userRole = UserRoles.Customer)
        {
            return await _context.RolePriviligae.Where(result => result.UserRole == userRole).ToListAsync();
        }

        // GET: api/RolePriviligaes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolePriviligae>> GetRolePriviligae(int id)
        {
            var rolePriviligae = await _context.RolePriviligae.FindAsync(id);

            if (rolePriviligae == null)
            {
                return NotFound();
            }

            return rolePriviligae;
        }

        // PUT: api/RolePriviligaes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRolePriviligae(int id, RolePriviligae rolePriviligae)
        {
            if (id != rolePriviligae.Id)
            {
                return BadRequest();
            }

            _context.Entry(rolePriviligae).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolePriviligaeExists(id))
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

        // POST: api/RolePriviligaes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RolePriviligae>> PostRolePriviligae(RolePriviligae rolePriviligae)
        {
            _context.RolePriviligae.Add(rolePriviligae);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetRolePriviligae", new { id = rolePriviligae.Id }, rolePriviligae);
        }

        // DELETE: api/RolePriviligaes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RolePriviligae>> DeleteRolePriviligae(int id)
        {
            var rolePriviligae = await _context.RolePriviligae.FindAsync(id);
            if (rolePriviligae == null)
            {
                return NotFound();
            }

            _context.RolePriviligae.Remove(rolePriviligae);
            await _context.SaveChangesAsync();

            return rolePriviligae;
        }

        private bool RolePriviligaeExists(int id)
        {
            return _context.RolePriviligae.Any(e => e.Id == id);
        }
    }
}
