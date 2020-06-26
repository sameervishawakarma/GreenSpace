using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
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
    public class ReservationController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _Configuration;

        public ReservationController(ReservationsDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _Configuration = configuration;
        }

        // GET: api/Reservation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservationModels(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.ReservationModels.ProjectTo<ReservationDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        // GET: api/Reservation/5
        [Route("api/Reservation/GetReservationModel")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationModel>> GetReservationModel(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var reservationModel = await _context.ReservationModels.FindAsync(id);

            if (reservationModel == null)
            {
                return NotFound();
            }

            return _mapper.Map<ReservationDto>(reservationModel);
        }

        // PUT: api/Reservation/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservationModel(long id, ReservationModel reservationModel)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(reservationModel.Key, _Configuration);
            if (id != reservationModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(reservationModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationModelExists(id, reservationModel.Key))
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

        // POST: api/Reservation
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ReservationModel>> PostReservationModel(ReservationModel reservationModel)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(reservationModel.Key, _Configuration);
            _context.ReservationModels.Add(reservationModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservationModel", new { id = reservationModel.Id, Key = reservationModel.Key }, reservationModel);
        }



        [HttpPut()]
        [Route("SetReservationApprove")]
        [Authorize(Policy = "OnlyAllAdmins")]
        //  [Authorize(Policy = "OnlySiteAdmin")]

        public async Task<ActionResult<ReservationDto>> SetReservationApprove(long id,string Key, ApproveStatus approve)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);

            var reservationModel = await _context.ReservationModels.FindAsync(id);


            if (reservationModel == null)
            {
                return NotFound();
            }

            reservationModel.Approved = approve;
            await _context.SaveChangesAsync();

            return _mapper.Map<ReservationDto>(reservationModel);
        }


        // DELETE: api/Reservation/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReservationModel>> DeleteReservationModel(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var reservationModel =  _context.ReservationModels.Include(model => model.FoodDetailItems).FirstOrDefault(model => model.Id == id);
            if (reservationModel == null)
            {
                return NotFound();
            }

            _context.RemoveRange(reservationModel.FoodDetailItems);

            _context.ReservationModels.Remove(reservationModel);
            await _context.SaveChangesAsync();

            return reservationModel;
        }



        private bool ReservationModelExists(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.ReservationModels.Any(e => e.Id == id);
        }
    }
}
