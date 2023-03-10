using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryContext _db;
        private readonly IMapper _mapper;

        public AuthorsController(LibraryContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _db.Authors.ToListAsync();
        }
        [HttpPost]
        public async Task<ActionResult<AuthorCreateDTO>> CreateAuthor([FromBody] AuthorCreateDTO authorCreate)
        {
            var map = _mapper.Map<Author>(authorCreate);
            await _db.AddAsync(map);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthors), new { author = authorCreate.AuthorName }, authorCreate);
        }
    }
}
