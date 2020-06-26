using System;
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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _Configuration;


        public ReportController(ReservationsDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _Configuration = configuration;

            //var optionsBuilder = new DbContextOptionsBuilder<ReservationsDbContext>();
            //string connectionstring = _Configuration.GetConnectionString("TestDbConn");
            //optionsBuilder.UseSqlServer(connectionstring);
            //_context1 = new ReservationsDbContext(optionsBuilder.Options);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationModel>>> GetReservationReportModels(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.ReservationModels.Include(model => model.Room).ThenInclude(floor => floor.Floor).ProjectTo<ReservationDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
        //return _context.ReservationModels.FromSqlInterpolated($"select RS.*,RM.Name as RoomName from ReservationModels as RS inner join Rooms as RM on RS.RoomId = RM.Id Inner join Floor as FL on RM.FloorId = FL.Id").ToList();
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var reservations = await _context.ReservationModels.Include(model => model.Room).ThenInclude(floor => floor.Floor).ProjectTo<ReservationDto>(_mapper.ConfigurationProvider).ToListAsync();
            var Rooms = await _context.Rooms.Include(room => room.Floor).Where(room=>room.Approved != ApproveRoomStatus.Disapproved).ToListAsync();
            foreach (ReservationModel RM in reservations)
            {
                var item = Rooms.Find(x => x.Id == RM.RoomId);
                Rooms.Remove(item);
            }
            return Rooms;
        }

        [HttpPost]
        public async Task<ActionResult> PostReportTemplate(ReportTemplate reporttemplate)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(reporttemplate.Key, _Configuration);
            _context.ReportTemplate.Add(reporttemplate);
            await _context.SaveChangesAsync();
            if (reporttemplate.GroupIds != null && reporttemplate.GroupIds.Length > 0)
            {
                var rt = await _context.ReportGroups.Where(report => report.ReportTemplateId == reporttemplate.Id).ToListAsync();
                if (rt != null)
                {
                    foreach (ReportGroups rg in rt)
                    {
                        _context.ReportGroups.Remove(rg);
                        await _context.SaveChangesAsync();
                    }

                }
                var ids = reporttemplate.GroupIds.Substring(1, reporttemplate.GroupIds.Length - 2).Split(',');
                foreach (string gid in ids)
                {
                    var rgroup = await _context.Groups.FindAsync(Convert.ToInt32(gid));
                    var reportgroup = new ReportGroups()
                    {
                        Name = rgroup.Name,
                        IsActive = true,
                        ReportTemplateId = reporttemplate.Id,
                        GroupId = rgroup.Id
                    };
                    _context.ReportGroups.Add(reportgroup);
                    await _context.SaveChangesAsync();
                }
            }

            if (reporttemplate.FieldIds != null && reporttemplate.FieldIds.Length > 0)
            {
                var rt = await _context.ReportFields.Where(report => report.ReportTemplateId == (int)reporttemplate.Id).ToListAsync();
                if (rt != null)
                {
                    foreach (ReportFields rg in rt)
                    {
                        _context.ReportFields.Remove(rg);
                        await _context.SaveChangesAsync();
                    }

                }
                var ids = reporttemplate.FieldIds.Substring(1, reporttemplate.FieldIds.Length - 2).Split(',');
                foreach (string gid in ids)
                {
                    var rfield = await _context.Fields.FindAsync(Convert.ToInt64(gid));
                    if (rfield != null)
                    {
                        var reportfield = new ReportFields()
                        {
                            Name = rfield.Name,
                            DataType = (int)rfield.DataType,
                            GroupsFilter = rfield.GroupsFilter,
                            FormsFilter = rfield.FormsFilter,
                            ParentType = (int)rfield.ParentType,
                            DisplayText = rfield.DisplayText,
                            ReportTemplateId = reporttemplate.Id,
                            FieldId = rfield.Id,
                        };
                        _context.ReportFields.Add(reportfield);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return CreatedAtAction("GetReportList", new { id = reporttemplate.Id, Key = reporttemplate.Key }, reporttemplate);
        }

        [HttpPost]
        public async Task<ActionResult> PostReportFields(ReportFields reportfields)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(reportfields.Key, _Configuration);
            _context.ReportFields.Add(reportfields);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> PostReportGroups(ReportGroups reportgroups)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(reportgroups.Key, _Configuration);
            _context.ReportGroups.Add(reportgroups);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public async Task<ReportTemplate> GetData(string Key, int id)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.ReportTemplate.SingleOrDefaultAsync(rt => rt.Id == id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportTemplate(long id, ReportTemplate reporttemplate)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(reporttemplate.Key, _Configuration);
            ReportTemplate rtt = await GetData(reporttemplate.Key, reporttemplate.Id);

            //
            if (id != reporttemplate.Id)
            {
                return BadRequest();
            }
            _context.Entry(reporttemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
                if (reporttemplate.GroupIds != null && reporttemplate.GroupIds.Length>0)
                {
                    if (rtt.GroupIds != reporttemplate.GroupIds)
                    {
                        var rt = await _context.ReportGroups.Where(report => report.ReportTemplateId == reporttemplate.Id).ToListAsync();
                        if (rt != null)
                        {
                            foreach (ReportGroups rg in rt)
                            {
                                _context.ReportGroups.Remove(rg);
                                await _context.SaveChangesAsync();
                            }

                        }
                        var ids = reporttemplate.GroupIds.Substring(1, reporttemplate.GroupIds.Length - 2).Split(',');
                        foreach (string gid in ids)
                        {
                            var rgroup = await _context.Groups.FindAsync(Convert.ToInt32(gid));
                            var reportgroup = new ReportGroups()
                            {
                                Name = rgroup.Name,
                                IsActive = true,
                                ReportTemplateId = reporttemplate.Id,
                                GroupId = rgroup.Id
                            };
                            _context.ReportGroups.Add(reportgroup);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                if (reporttemplate.FieldIds != null && reporttemplate.FieldIds.Length > 0)
                {
                    if (rtt.FieldIds != reporttemplate.FieldIds)
                    {
                        var rt = await _context.ReportFields.Where(report => report.ReportTemplateId == (int)reporttemplate.Id).ToListAsync();
                        if (rt != null)
                        {
                            foreach (ReportFields rg in rt)
                            {
                                _context.ReportFields.Remove(rg);
                                await _context.SaveChangesAsync();
                            }

                        }
                        var ids = reporttemplate.FieldIds.Substring(1, reporttemplate.FieldIds.Length - 2).Split(',');
                        foreach (string gid in ids)
                        {
                            var rfield = await _context.Fields.FindAsync(Convert.ToInt64(gid));
                            if (rfield != null)
                            {
                                var reportfield = new ReportFields()
                                {
                                    Name = rfield.Name,
                                    DataType = (int)rfield.DataType,
                                    GroupsFilter = rfield.GroupsFilter,
                                    FormsFilter = rfield.FormsFilter,
                                    ParentType = (int)rfield.ParentType,
                                    DisplayText = rfield.DisplayText,
                                    ReportTemplateId = reporttemplate.Id,
                                    FieldId = rfield.Id,
                                };
                                _context.ReportFields.Add(reportfield);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(reporttemplate.Id,reporttemplate.Key))
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<ReportTemplate>> DeleteReportTemplate(int id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var reporttemplate = await _context.ReportTemplate.FindAsync(id);
            if (reporttemplate == null)
            {
                return NotFound();
            }

            _context.ReportTemplate.Remove(reporttemplate);
            await _context.SaveChangesAsync();

            return reporttemplate;
        }

        private bool ReportExists(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.ReportTemplate.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupsModel>>> GetGroupsList(string Key)
        {
            if (Key != "")
            {
                ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
                return await _context.Groups.ToListAsync();
            }
            else
            {
                return await _context.Groups.ToListAsync();
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportTemplate>>> GetReportList(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.ReportTemplate.Include(Report=>Report.RFields).Include(Report=>Report.RGroups).Include(report=>report.RParameter).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReportTemplate>> GetReportList(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.ReportTemplate.Include(Report => Report.RFields).Include(Report => Report.RGroups).Include(report => report.RParameter).SingleOrDefaultAsync(Report => Report.Id == id);
        }

    }
}