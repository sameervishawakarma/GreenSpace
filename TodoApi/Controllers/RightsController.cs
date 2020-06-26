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
    public class RightsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;

        public RightsController(ReservationsDbContext context)
        {
            _context = context;
        }

        // GET: api/Rights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RightMaster>>> GetRightMaster(string module)
        {
            return await _context.RightMaster.Include(right=>right.DFields).Where(right=>right.Module==module).ToListAsync();
        }

        // GET: api/Rights/GetCategoryList
        [HttpGet]
        [Route("GetCategoryList")]
        public ActionResult<List<string>> GetCategoryList()
        {
            List<string> categories = _context.RightMaster.Select(m => m.Module).Distinct().ToList();
            return categories;
        }

        // GET: api/Rights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RightMaster>> GetRightMaster(int id)
        {
            var rightMaster = await _context.RightMaster.Include(right => right.DFields).SingleOrDefaultAsync(right=>right.Id==id);

            if (rightMaster == null)
            {
                return NotFound();
            }

            return rightMaster;
        }

        // PUT: api/Rights/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRightMaster(int id, RightMaster rightMaster)
        {
            if (id != rightMaster.Id)
            {
                return BadRequest();
            }

            _context.Entry(rightMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RightMasterExists(id))
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

        // POST: api/Rights
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RightMaster>> PostRightMaster(RightMaster rightMaster)
        {
            _context.RightMaster.Add(rightMaster);
            await _context.SaveChangesAsync();
            var RightDetail = new RightDetail()
            {
                RightMasterId = rightMaster.Id,
                View = true,
                List = true,
                Add = true,
                Edit = true,
                Delete = true
            };
            _context.RightDetail.Add(RightDetail);
            await _context.SaveChangesAsync();

            string[] Roles = GetUserRoles();
            for (int i=0;i<Roles.Length;i++)
            {
                var rolepri = new RolePriviligae()
                {
                    PageId = rightMaster.Id,
                    UserRole = (UserRoles)i,
                    View = true,
                    List = true,
                    Add = true,
                    Edit = true,
                    Delete = true
                };
                _context.RolePriviligae.Add(rolepri);
                await _context.SaveChangesAsync();
            }
            
            return CreatedAtAction("GetRightMaster", new { id = rightMaster.Id }, rightMaster);
        }

        // DELETE: api/Rights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RightMaster>> DeleteRightMaster(int id)
        {
            var rightMaster = await _context.RightMaster.FindAsync(id);
            if (rightMaster == null)
            {
                return NotFound();
            }

            _context.RightMaster.Remove(rightMaster);
            await _context.SaveChangesAsync();

            return rightMaster;
        }

        private bool RightMasterExists(int id)
        {
            return _context.RightMaster.Any(e => e.Id == id);
        }

        private string[] GetUserRoles()
        {
            return new string[] { "Customer", "Admin", "SuperAdmin", "SiteAdmin" };
        }
    }
}
