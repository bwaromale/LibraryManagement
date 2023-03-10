using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _db;
        private readonly IMapper _mapper;

        public BooksController(LibraryContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = _db.Books.ToListAsync();
            return await books;
        }
        [HttpPost]
        public async Task<ActionResult<BookCreateDTO>> BookCreate([FromBody] BookCreateDTO bookCreate)
        {
            var map = _mapper.Map<Book>(bookCreate);
            await _db.AddAsync(map);
            await _db.SaveChangesAsync();

            return Ok(map);
        }
    }
}
