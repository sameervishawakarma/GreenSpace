using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.DTO;
using TodoApi.Helpers;
using TodoApi.Models;
using Image = TodoApi.Models.Image;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _Configuration;


        public RoomsController(ReservationsDbContext context, IHostingEnvironment hostingEnvironment , IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            this._environment = _environment;
            _Configuration = configuration;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoom(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.Rooms.Include(location => location.ResourceType).Include(location => location.FieldValues).ThenInclude(values => values.Field).ProjectTo<RoomDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet]
        [Route("GetRoomFromStatus")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomFromStatus(string Key,ApproveRoomStatus approved=ApproveRoomStatus.Approved)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.Rooms.Include(location => location.ResourceType).Include(location => location.FieldValues).ThenInclude(values => values.Field).ProjectTo<RoomDto>(_mapper.ConfigurationProvider).Where(room=>room.Approved==approved).ToListAsync();
        }

        // GET: api/Rooms
        [HttpPost]
        [Route("FilterRooms")]
        [ProducesResponseType(typeof(IEnumerable<Room>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<IEnumerable<RoomDto>>> FilterRooms(Filter filter)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(filter.Key, _Configuration);
            //validate filter
            if (filter == null) return BadRequest("You should provide filter");
            if (filter.Fields != null)
            {
                if (filter.Fields.Count == 0) return BadRequest("You should provide some fields");
            }

            try
            {
                var res = FilterRoomsBll(filter);
                return Ok(res);
            }
            catch (IncorrectFieldFilterException e)
            {
                return BadRequest(e.Message);
            }
          


            //field filtration
            /*foreach (var keyValue in filter.Fields)
            {
                var prop = keyValue.Name;
                var val = keyValue.Value;
                res.Where(c => c.GetType().GetProperty(prop).Name == prop && c.GetType().GetProperty(prop).GetValue());
            }*/



            /*var all = await _context.Rooms.Include(location => location.FieldValues).ThenInclude(values => values.Field).ToListAsync();
            return all.Where((model, i) => filter.FloorIds.Contains(model.FloorId)).ToList();*/
           
        }

        //todo: refactor to BLL
        private IEnumerable<RoomDto> FilterRoomsBll(Filter filter)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(filter.Key, _Configuration);
            var roomsfromRegions = _context.Regions
                .Include(region => region.Sites)
                    .ThenInclude(site => site.Buildings)
                        .ThenInclude(building => building.Floors)
                            .ThenInclude(floor => floor.Rooms)
                                .ThenInclude(room => room.Images)
                .Include(region => region.Sites)
                    .ThenInclude(site => site.Buildings)
                        .ThenInclude(building => building.Floors)
                            .ThenInclude(floor => floor.Rooms)
                                .ThenInclude(room => room.ResourceType)

                .ToList()
                .Where((region, i) => filter.RegionIds?.Contains(region.Id) ?? true).SelectMany(region =>
                    region.Sites.SelectMany(site =>
                        site.Buildings.SelectMany(building => building.Floors.SelectMany(floor => floor.Rooms)))).ToList();

            var roomsfromSites = _context.Sites
                .Include(site => site.Buildings)
                    .ThenInclude(building => building.Floors)
                        .ThenInclude(floor => floor.Rooms)
                            .ThenInclude(room => room.Images)
                .Include(site => site.Buildings)
                    .ThenInclude(building => building.Floors)
                        .ThenInclude(floor => floor.Rooms)
                            .ThenInclude(room => room.ResourceType)
                .ToList()
                .Where((site, i) => filter.SiteIds?.Contains(site.Id) ?? true).SelectMany(site =>
                    site.Buildings.SelectMany(building => building.Floors.SelectMany(floor => floor.Rooms))).ToList();

            var roomsfromBuildings = _context.Building
                .Include(building => building.Floors)
                    .ThenInclude(floor => floor.Rooms)
                        .ThenInclude(room => room.Images)
                .Include(building => building.Floors)
                    .ThenInclude(floor => floor.Rooms)
                        .ThenInclude(room => room.ResourceType)
                .ToList()
                .Where((site, i) => filter.BuildingIds?.Contains(site.Id) ?? true)
                .SelectMany(building => building.Floors.SelectMany(floor => floor.Rooms)).ToList();

            var roomsfromFloors = _context.Floor
                .Include(floor => floor.Rooms)
                .ThenInclude(room => room.Images)
                .Include(floor => floor.Rooms)
                .ThenInclude(room => room.ResourceType)
                .ToList()
                .Where((site, i) => filter.FloorIds?.Contains(site.Id) ?? true).SelectMany(floor => floor.Rooms).ToList();



               var res = roomsfromRegions.Union(roomsfromSites).Union(roomsfromBuildings).Union(roomsfromFloors).AsQueryable().ProjectTo<RoomDto>(_mapper.ConfigurationProvider).ToList();
              
          //  var res = roomsfromRegions.Union(roomsfromSites).Union(roomsfromBuildings).Union(roomsfromFloors).AsEnumerable();


            if (filter.Fields != null)
            {
                try
                {
                    //res.Where((dto, i) => dto.HasDockingStation == true);

                    var expressionTree = ExpressionHelper.ExpressionHelper.ConstructAndExpressionTree<RoomDto>(filter.Fields);
                    var anonymousFunc = expressionTree.Compile();
                    res = res.AsEnumerable().Where(anonymousFunc).ToList();
                }

                catch (ArgumentException e)
                {
                    throw new IncorrectFieldFilterException("Seems like Fields contain incorrect fieldname or value");
                 //  return BadRequest("Seems like Fields contain incorrect fieldname or value");
               }
            }

            return res;
        }

        [HttpPost]
        [Route("GetReservations")]
        
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations(Filter filter, DateTime startDate, DateTime endDate)
        {
            //validate filter
            ReservationsDbContext _context = DBChange.DBaseChange(filter.Key, _Configuration);
            if (filter == null) return BadRequest("You should provide filter");
            if (filter.Fields != null)
            {
                if (filter.Fields.Count == 0) return BadRequest("You should provide some fields");
            }

            try
            {
                var res = FilterRoomsBll(filter);
                var roomIds = res.Select(room => room.Id);
                var reservations = await _context.ReservationModels.Include(model => model.FoodDetailItems)
                    .Where(model => model.StartTime > startDate && model.EndTime < endDate)
                    .Where(model => roomIds.Contains(model.RoomId)).ProjectTo<ReservationDto>(_mapper.ConfigurationProvider).ToListAsync();
                return reservations;
            }
            catch (IncorrectFieldFilterException e)
            {
                return BadRequest(e.Message);
            }


            /*var mthd =  FilterRooms(filter);



            if (res == null) return Ok();
            var roomIds = res.Value.ToList().Select(room => room.Id);
            var reservations = await _context.ReservationModels
                .Where(model => model.StartTime > startDate && model.EndTime < endDate)
                .Where(model => roomIds.Contains(model.Id)).ProjectTo<ReservationDto>(_mapper.ConfigurationProvider).ToListAsync();
            return reservations;

              */
            return Ok();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Room), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RoomDto>> GetRoom(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var roomModel = await _context.Rooms.Include(room => room.Images)
                .Include(location => location.ResourceType)
                .Include(location => location.FieldValues)
                .ThenInclude(values => values.Field)
                .ProjectTo<RoomDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(location1 => location1.Id == id);

            if (roomModel == null)
            {
                return NotFound();
            }

            return roomModel;
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        // [Authorize(Policy = "OnlyCompanyAdmin")]
        public async Task<IActionResult> PutRoom(long id, Room roomModel)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(roomModel.Key, _Configuration);
            if (id != roomModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id,roomModel.Key))
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

        [HttpPut]
        [Route("RoomActive")]
        public async Task<IActionResult> RoomActive(Room roomModel)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(roomModel.Key, _Configuration);            
            _context.Entry(roomModel).Property(x => x.Approved).IsModified = true;// .State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(roomModel.Id, roomModel.Key))
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

        // POST: api/Rooms
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(200)]
        //  [Authorize(Policy = "OnlyCompanyAdmin")]
        //    public async Task<ActionResult<RoomDto>> PostRoom([FromForm(Name = "file")][AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]IFormFile file, Room roomModel)
          public async Task<ActionResult<RoomDto>> PostRoom([FromForm]RoomWithImage roomWithImage)

        //  public async Task<ActionResult<RoomDto>> PostRoom(IFormFile file, Room roomModel)
      //    public async Task<ActionResult<RoomDto>> PostRoom([FromForm(Name = "file")][AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]IFormFile file, [FromForm]string roomModelStr)

        {
          /*var  roomModel = roomWithImage.Room;
          var file = roomWithImage.File;*/

          var roomModel = JsonConvert.DeserializeObject<Room>(roomWithImage.RoomStr);
          var files = roomWithImage.Files;
            //ReservationsDbContext _context = DBChange.DBaseChange(roomWithImage.Key, _Configuration);
            _context.Rooms.Add(roomModel);
            await _context.SaveChangesAsync();


            if (files?.Count != 0)
            {
                foreach (var file in files)
                {
                    await UploadFile(file, roomModel, "In");
                }
            }

            /* if there would be files
             if (files != null)
            {
                foreach (var formFile in files)
                {
                   await PostImage(formFile, roomModel.Id);
                }
            }*/

            return CreatedAtAction("GetRoom", new { id = roomModel.Id }, _mapper.Map<RoomDto>(roomModel));
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        //  [Authorize(Policy = "OnlyCompanyAdmin")]
        public async Task<ActionResult<Room>> DeleteRoom(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var roomModel = await _context.Rooms.FindAsync(id);
            if (roomModel == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(roomModel);
            await _context.SaveChangesAsync();

            return roomModel;
        }


      public class EmailForm
      {
          [Display(Name = "Add a picture")]
          [DataType(DataType.Upload)]
          [FileExtensions(Extensions = "jpg,png,gif,jpeg,bmp,svg")]
          public IFormFile SubmitterPicture { get; set; }
      }

        [HttpPost]
        [Route("PostImage")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        // public async Task<IActionResult> PostImage([FileExtensions(Extensions = "jpg,png,gif,jpeg,bmp,svg")]IFormFile file, long id)
        public async Task<IActionResult> PostImage([AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]ICollection<IFormFile> file, long id)
        {
            var room = _context.Rooms.Include(room1 => room1.Images).SingleOrDefault(room2 => room2.Id == id);
            if (room == null) return BadRequest("Room not found");

            foreach (var fil in file)
            {
                await UploadFile(fil, room, "Up");
            }
                

            return Ok(_mapper.Map<RoomDto>(room));
          //  return Ok(new { count = files.Count });
      }

        private async Task UploadFile(IFormFile file, Room room, string op)
        {
            //var fileName = Path.GetFileName(file.FileName);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, fileName);

            var fileNameRes = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePathRes = Path.Combine(_hostingEnvironment.ContentRootPath, fileNameRes);

            var fileNameTn = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePathTn = Path.Combine(_hostingEnvironment.ContentRootPath, fileNameTn);

            string filePathFull;
            string filePathThumbnail;

            // Bitmap bmp;
            await using (var fileSteam = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileSteam);
                //  fileSteam.Position = 0;
                //   bmp = new Bitmap(fileSteam);
            }


            //resize if needed
            filePathFull = ImageHelper.ResizeImage(filePath, filePathRes, false);
            filePathThumbnail = ImageHelper.ResizeImage(filePath, filePathTn, true);
            //make thumbnail

            //var image = new Image()
            //{
            //    RoomId= room.Id,
            //    Path = $"{fileName}",
            //    Name = file.FileName,
            //    IsThumbnail = true,
            //    PathToFullImage = fileName
            //};

            //room.Images.Add(new Image()
            //{
            //    Path = $"{fileName}",
            //    Name = file.FileName
            //});
            //room.Images.Add(new Image()
            //{
            //    Name = file.FileName,
            //    Path = $"{fileNameTn}",
            //    IsThumbnail = true,
            //    PathToFullImage = fileName
            //});
            
            ReservationsDbContext _context = DBChange.DBaseChange(room.Key, _Configuration);
            if (op == "In")
            {
                var image = new Image()
                {
                    RoomId = room.Id,
                    Path = $"{fileName}",
                    Name = file.FileName,
                    IsThumbnail = true,
                    PathToFullImage = fileName
                };
                _context.Image.Add(image);
            }
            else
            {
                Int64 Id=0;
                foreach (var img in room.Images)
                {
                    Id = img.Id;
                }
                
                var image1 = new Image()
                {
                    Id = Id,
                    RoomId = room.Id,
                    Path = $"{fileName}",
                    Name = file.FileName,
                    IsThumbnail = true,
                    PathToFullImage = fileName
                };
                _context.Entry(image1).State = EntityState.Modified;
            }
            _context.SaveChanges();
        }
        private bool RoomExists(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.Rooms.Any(e => e.Id == id);
        }

        public class RoomWithImage
        {
            
            [FromForm] public string RoomStr { get; set; }

            [FromForm]
            [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
            //public Microsoft.AspNetCore.Http.IFormFile Files { get; set; }
            //for multiple images
            public ICollection<Microsoft.AspNetCore.Http.IFormFile> Files { get; set; }
        }
    }

    internal class IncorrectFieldFilterException : Exception
    {
        public IncorrectFieldFilterException(string seemsLikeFieldsContainIncorrectFieldnameOrValue) : base(seemsLikeFieldsContainIncorrectFieldnameOrValue)
        {
          
        }
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _Extensions;
        public AllowedExtensionsAttribute(string[] Extensions)
        {
            _Extensions = Extensions;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            //Multiple images
            var files = value as ICollection<IFormFile>;
            //var file = value as IFormFile;
            foreach (var file in files)
            {
                if (!(file == null))
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (!_Extensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
            }
            return ValidationResult.Success;
        }

      

        public string GetErrorMessage()
        {
            return $"This photo extension is not allowed!";
        }
    }
}
