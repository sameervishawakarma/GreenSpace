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
    public class RegionsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IConfiguration _Configuration;

        public RegionsController(ReservationsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        // GET: api/Regions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Region>>> GetRegions(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.Regions.Include(region => region.Sites).ToListAsync();
        }

        // GET: api/Regions/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Region>> GetRegion(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var region = await _context.Regions.Include(region => region.Sites).SingleOrDefaultAsync(region1 => region1.Id == id);

            if (region == null)
            {
                return NotFound();
            }

            return region;
        }

        // PUT: api/Regions/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutRegion(long id, Region region)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(region.Key, _Configuration);
            if (id != region.Id)
            {
                return BadRequest();
            }

            _context.Entry(region).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionExists(id, region.Key))
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

        // POST: api/Regions
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(200)]
       
        public async Task<ActionResult<Region>> PostRegion(Region region)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(region.Key, _Configuration);
            _context.Regions.Add(region);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegion", new { id = region.Id, Key = region.Key }, region);
        }

        // DELETE: api/Regions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Region>> DeleteRegion(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var region = await _context.Regions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();

            return region;
        }

        private bool RegionExists(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.Regions.Any(e => e.Id == id);
        }
    }
}
