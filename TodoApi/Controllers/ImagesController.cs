using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
    public class ImagesController : ControllerBase
    {
        private readonly ReservationsDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _Configuration;

        public ImagesController(ReservationsDbContext context, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _Configuration = configuration;
        }

        // GET: api/Images
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Image>>> GetImage(string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return await _context.Image.ToListAsync();
        }

        // GET: api/Images/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Image>> GetImage(long id,string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var image = await _context.Image.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            return image;
        }

        // DELETE: api/Images/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<Image>> DeleteImage(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            _context.Image.Remove(image);
            await _context.SaveChangesAsync();

            //delete file
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, image.Name);

            var fileInfo = new System.IO.FileInfo(filePath);
            fileInfo.Delete();


            return image;
        }

        private bool ImageExists(long id, string Key)
        {
            ReservationsDbContext _context = DBChange.DBaseChange(Key, _Configuration);
            return _context.Image.Any(e => e.Id == id);
        }
    }
}
