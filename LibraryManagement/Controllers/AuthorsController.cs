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
        private readonly IAuthorRepository _authorServ;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public AuthorsController(IAuthorRepository authorServ, IMapper mapper)
        {
            _authorServ = authorServ;
            _mapper = mapper;
            this._response = new();
        }
        [HttpGet]
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
        [HttpGet("{id}/books")]
        public async Task<ActionResult<APIResponse>> GetBooksAttachedtoAuthor(int id)
        {
            try
            {
                var books = await _authorServ.GetBooksAttachedtoAuthor(id);
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
