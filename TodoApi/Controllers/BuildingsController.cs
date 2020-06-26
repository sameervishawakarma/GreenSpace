using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TodoApi.DTO;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _Configuration;

        public BuildingsController(ReservationsDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _Configuration = configuration;
        }

        // GET: api/Buildings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuilding(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.Building.Include(building => building.Floors).ProjectTo<BuildingDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        // GET: api/Buildings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Building>> GetBuilding(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var building = await _context.Building.Include(building => building.Floors).ProjectTo<BuildingDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(building1 => building1.Id == id);

            if (building == null)
            {
                return NotFound();
            }

            return building;
        }

        // PUT: api/Buildings/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuilding(long id, Building building)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(building.Key, _Configuration);
            if (id != building.Id)
            {
                return BadRequest();
            }

            _context.Entry(building).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildingExists(id, building.Key))
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

        // POST: api/Buildings
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Building>> PostBuilding(Building building)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(building.Key, _Configuration);
            _context.Building.Add(building);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBuilding", new { id = building.Id, Key = building.Key }, building);
        }

        // DELETE: api/Buildings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Building>> DeleteBuilding(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var building = await _context.Building.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            _context.Building.Remove(building);
            await _context.SaveChangesAsync();

            return building;
        }

        private bool BuildingExists(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.Building.Any(e => e.Id == id);
        }
    }
}
