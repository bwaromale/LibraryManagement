using AutoMapper;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository _bookServ;
        private readonly IAuthorRepository _authorServ;
        private readonly IMapper _mapper;
        protected readonly APIResponse _response;
        public BooksController(IBooksRepository bookServ,IAuthorRepository authorServ, IMapper mapper)
        {
            _bookServ = bookServ;
            _authorServ = authorServ;
            _mapper = mapper;
            _response = new APIResponse();
        }
        [HttpGet]
        [Authorize(Roles ="User, Approver, Admin")]
        public async Task<ActionResult<APIResponse>> GetBooks()
        {
            try
            {
                var books = await _bookServ.GetAllAsync();
                var booksWithAuthors = new List<BookWithAuthorDTO>();
                foreach(var book in books)
                {
                    var author = await _authorServ.GetAsync(a=> a.AuthorId == book.AuthorId);
                    var bookWithAuthor = _mapper.Map<BookWithAuthorDTO>(book);
                    bookWithAuthor.Author = _mapper.Map<AuthorDTO>(author);
                    booksWithAuthors.Add(bookWithAuthor);
                }
                _response.Result = booksWithAuthors;
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
        [HttpGet("{bookName}")]
        [Authorize(Roles = "User, Approver, Admin")]
        public async Task<ActionResult<APIResponse>> GetBook(string bookName)
        {
            try
            {
                var book = await _bookServ.GetAsync(b => b.Title == bookName);
                if (book == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { "Not Found" };
                    return NotFound(_response);
                }
                var author = await _authorServ.GetAsync(a=>a.AuthorId == book.AuthorId);

                var mapBook  = _mapper.Map<BookWithAuthorDTO>(book);
                mapBook.Author = _mapper.Map<AuthorDTO>(author);
                
                _response.Result = mapBook;
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
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<APIResponse>> BookCreate([FromBody] BookUpsertDTO bookCreate)
        {
            try
            {
                var book = _mapper.Map<Book>(bookCreate);
                var bookExist = await _bookServ.CheckDuplicateAtCreation(check => check.Title==bookCreate.Title);
                if(bookExist)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() {$"A book with title '{book.Title}' already exist." };
                    return BadRequest(_response);
                }
                await _bookServ.CreateAsync(book);

                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = _mapper.Map<BookUpsertDTO>(book);

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
        [HttpDelete("{bookName}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<APIResponse>> DeleteBook(string bookName)
        {
            try
            {
                if (string.IsNullOrEmpty(bookName))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { $"{bookName} is an invalid input" };
                    return BadRequest(_response);
                }
                var book = await _bookServ.GetAsync(b => b.Title == bookName);
                if(book == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { $"Book with name '{bookName}' not found" };
                    return NotFound(_response);
                }
                await _bookServ.RemoveAsync(b =>b.Title == bookName);
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
        [HttpPut("{bookName}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<APIResponse>> UpdateBook(string bookName, [FromBody] BookUpsertDTO bookCreateDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(bookName) || bookCreateDTO == null) 
                { 
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Invalid input" };
                    return BadRequest(_response);
                }
                var book = await _bookServ.GetAsync(b=>b.Title == bookName);
                if (book == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { $"'{bookName}' Not found" };
                    return NotFound(_response);
                }
                book.Title = bookCreateDTO.Title;
                book.CoverType = bookCreateDTO.CoverType;
                book.NoOfPages = bookCreateDTO.NoOfPages;
                book.ForewardBy = bookCreateDTO.ForewardBy;
                book.Price = bookCreateDTO.Price;
                book.Available = bookCreateDTO.Available;
                book.AuthorId = bookCreateDTO.AuthorId;

                await _bookServ.UpdateAsync(book);
                
                _response.StatusCode=HttpStatusCode.OK;
                _response.Result = bookCreateDTO;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string>() { ex.Message.ToString() };
                return BadRequest(_response);
            }
        }
    }
}
