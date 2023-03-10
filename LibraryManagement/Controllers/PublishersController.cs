using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly LibraryContext _db;
        private readonly IMapper _mapper;

        public PublishersController(LibraryContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            return await _db.Publishers.ToListAsync();

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await _db.Publishers.FindAsync(id);
            if(publisher == null)
            {
                return NotFound();
            }
            return Ok(publisher);
        }
        [HttpPost]
        public async Task<ActionResult<PublisherCreateDTO>> PostPublisher([FromBody] PublisherCreateDTO publisher)
        {
            
            
            var map = _mapper.Map<Publisher>(publisher);
            await _db.AddAsync(map);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPublishers), new { name = publisher.PublisherName }, publisher);
        }
    }
}
