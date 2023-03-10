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
           var authors = await _db.Authors.ToListAsync();
            return Ok(authors);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return author;
        }
        [HttpGet("{id}/books")]
        public async Task<ActionResult<Book>> GetBooksAttachedtoAuthor(int id)
        {
            var books = await _db.Books.Where(b => b.AuthorId == id).ToListAsync();
            if (books.Count == 0)
            {
                return NotFound();
            }
            return Ok(books);
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
