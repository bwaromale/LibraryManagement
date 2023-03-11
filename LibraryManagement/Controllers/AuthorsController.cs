using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _db;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public AuthorsController(IAuthorRepository db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAuthors()
        {
            try
            {
                IEnumerable<Author> authors = await _db.GetAllAsync();
                _response.Result = _mapper.Map<IEnumerable<AuthorDTO>>(authors);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
            
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetAuthor(int id)
        {
            try
            {
                var author = await _db.GetAsync(id);
                if(author == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "Not Found" };
                    return NotFound(_response);
                }
                
                _response.Result = _mapper.Map<AuthorDTO>(author);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() {ex.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpGet("{id}/books")]
        public async Task<ActionResult<APIResponse>> GetBooksAttachedtoAuthor(int id)
        {
            try
            {
                var books = await _db.GetBooksAttachedtoAuthor(id);
                if (books == null || !books.Any())
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "Not Found" };
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<IEnumerable<BookDTO>>(books);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages= new List<string>() {ex.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateAuthor([FromBody] AuthorCreateDTO authorCreate)
        {
            try
            {
                var map = _mapper.Map<Author>(authorCreate);
                await _db.CreateAsync(map);
                _response.Result = _mapper.Map<AuthorDTO>(map);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtAction(nameof(GetAuthors), new { author = authorCreate.AuthorName }, _response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() {ex.ToString() };
                return BadRequest(_response);
            }
        }
    }
}
