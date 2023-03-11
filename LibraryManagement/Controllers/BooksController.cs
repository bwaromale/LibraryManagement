using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IRepository<Book> _db;
        private readonly IMapper _mapper;
        protected readonly APIResponse _response;
        public BooksController(IRepository<Book> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new APIResponse();
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetBooks()
        {
            try
            {
                var books = await _db.GetAllAsync();
                _response.Result = _mapper.Map<IEnumerable<BookDTO>>(books);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetBook(int id)
        {
            try
            {
                var book = await _db.GetAsync(id);
                if (book == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Not Found" };
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<BookDTO>(book);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> BookCreate([FromBody] BookCreateDTO bookCreate)
        {
            try
            {
                var book = _mapper.Map<Book>(bookCreate);
                var bookExist = await _db.CheckDuplicateAtCreation(check => check.Title==bookCreate.Title);
                if(bookExist)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() {$"A book with title '{book.Title}' already exist." };
                    return BadRequest(_response);
                }
                await _db.CreateAsync(book);

                _response.Result = _mapper.Map<BookCreateDTO>(book);
                return CreatedAtAction(nameof(GetBooks), new {b = bookCreate.Title}, _response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }
    }
}
