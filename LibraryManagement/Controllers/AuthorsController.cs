using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorServ;
        private readonly IBooksRepository _bookServ;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public AuthorsController(IAuthorRepository authorServ, IBooksRepository bookServ, IMapper mapper)
        {
            _authorServ = authorServ;
            _bookServ = bookServ;
            _mapper = mapper;
            this._response = new();
        }
        [HttpGet]
        [Authorize(Roles ="User, Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetAuthors()
        {
            try
            {
                IEnumerable<Author> authors = await _authorServ.GetAllAsync();
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
        [HttpGet("{authorName}")]
        [Authorize(Roles = "User, Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetAuthor(string authorName)
        {
            try
            {
                var author = await _authorServ.GetAsync(a => a.AuthorName == authorName);
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
        [HttpGet("{authorId}/books")]
        [Authorize(Roles = "User, Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetBooksAttachedtoAuthor(int authorId)
        {
            try
            {
                var author = await _authorServ.GetAsync(a => a.AuthorId == authorId);
                if (author == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "Not Found" };
                    return NotFound(_response);
                }
                var authorBooks = await _bookServ.GetBooksAttachedtoAuthor(author.AuthorId);
                var authorWithBooks = new AuthorWithBooksDTO()
                {
                    AuthorName = author.AuthorName,
                    Books = authorBooks.Select(b=>new BookDTO() 
                    { 
                        Title = b.Title,
                        CoverType = b.CoverType,
                        NoOfPages = b.NoOfPages,
                        ForewardBy = b.ForewardBy,
                        ISBN = b.ISBN,
                        Price = b.Price,
                        Available = b.Available,
                        TotalCopies = b.TotalCopies,
                        BorrowedCopies = b.BorrowedCopies
                    }
                    ).ToList()
                };
                
                _response.Result = authorWithBooks;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages= new List<string>() {ex.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResponse>> CreateAuthor([FromBody] AuthorUpsertDTO authorCreate)
        {
            try
            {
                var map = _mapper.Map<Author>(authorCreate);
                await _authorServ.CreateAsync(map);
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
        [HttpDelete("{authorName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResponse>> DeleteAuthor(string authorName)
        {
            try
            {
                if (string.IsNullOrEmpty(authorName))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Invalid input"};
                    return BadRequest(_response);
                }
                var author = await _authorServ.GetAsync(a => a.AuthorName == authorName);
                if(author == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { $"Author with name '{authorName}' not found" };
                    return NotFound(_response);
                }
                await _authorServ.RemoveAsync(a => a.AuthorName == authorName);
                _response.StatusCode=HttpStatusCode.OK;
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
        [HttpPut("{authorName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResponse>> UpdateAuthor(string authorName, [FromBody] AuthorUpsertDTO authorCreateDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(authorName) || authorCreateDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { "Invalid input" };
                    return BadRequest(_response);
                }

                var author = await _authorServ.GetAsync(a => a.AuthorName == authorName);
                
                if(author == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { $"Author with name '{authorName}' not found" };
                    return NotFound(_response);
                }

                author.AuthorName = authorCreateDTO.AuthorName;
                author.PublisherId = authorCreateDTO.PublisherId;
                author.UpdatedTime = DateTime.Now;

                await _authorServ.UpdateAsync(author);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = authorCreateDTO;
                return Ok(_response);
            }
            catch(Exception ex) {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages= new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }
    }
}
