using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TodoApi.DTO;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FieldsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IConfiguration _Configuration;

        public FieldsController(ReservationsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        // GET: api/Fields
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Field>>> GetFields(string Key, ParentType parentType = ParentType.NotSet)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            if (parentType != ParentType.NotSet)
            {
                //todo: seems like ef core can convert comparasion of enum to sql (sic!) so doing it on client for now
                // return  _context.Fields.Where((field, i) => field.ParentType == parentType).ToListAsync();
                return _context.Fields.AsEnumerable().Where((field, i) => field.ParentType == parentType).ToList();
            }
            else return await _context.Fields.ToListAsync();
        }

        [HttpGet("{Key}")]
        public async Task<ActionResult<IEnumerable<Field>>> GetFieldsFGroup(string Key, ParentType parentType = ParentType.NotSet)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);

            if (parentType != ParentType.NotSet)
            {
                //todo: seems like ef core can convert comparasion of enum to sql (sic!) so doing it on client for now
                // return  _context.Fields.Where((field, i) => field.ParentType == parentType).ToListAsync();
                return _context.Fields.AsEnumerable().Where((field, i) => field.ParentType == parentType).ToList();
            }
            else return await _context.Fields.ToListAsync();
        }

        // GET: api/Fields/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Field>> GetField(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var @field = await _context.Fields.FindAsync(id);

            if (@field == null)
            {
                return NotFound();
            }

            return @field;
        }

        // PUT: api/Fields/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        // [Authorize(Policy = "OnlyCompanyAdmin")]
        public async Task<IActionResult> PutField(long id, Field @field)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(@field.Key, _Configuration);
            if (id != @field.Id)
            {
                return BadRequest();
            }

            _context.Entry(@field).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldExists(id, @field.Key))
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

        // POST: api/Fields
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(200)]
        //  [Authorize(Policy = "OnlyCompanyAdmin")]
        public async Task<ActionResult<Field>> PostField(Field @field)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(@field.Key, _Configuration);
            _context.Fields.Add(@field);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetField", new { id = @field.Id }, @field);
        }

        // DELETE: api/Fields/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        //  [Authorize(Policy = "OnlyCompanyAdmin")]
        public async Task<ActionResult<Field>> DeleteField(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var @field = await _context.Fields.FindAsync(id);
            if (@field == null)
            {
                return NotFound();
            }

            _context.Fields.Remove(@field);
            await _context.SaveChangesAsync();

            return @field;
        }

        private bool FieldExists(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.Fields.Any(e => e.Id == id);
        }
    }
}
