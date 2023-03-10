using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisher _db;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public PublishersController(IPublisher db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetPublishers()
        {
            try
            {
                IEnumerable<Publisher> publishers = await _db.GetAllAsync();
                _response.Result = _mapper.Map<IEnumerable<PublisherDTO>>(publishers);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetPublisher(int id)
        {
            var publisher = await _db.GetAsync(id);
            _response.Result = _mapper.Map<PublisherDTO>(publisher);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        [HttpGet("{id}/authors")]
        public async Task<ActionResult<APIResponse>> GetAuthorsAttachedtoPublisher(int id)
        {
            var authors = await _db.GetAuthorsAttachedtoPublisher(id);
            if (!authors.Any())
            {
                return NotFound();
            }
            _response.Result = _mapper.Map<IEnumerable<AuthorDTO>>(authors);
            return Ok(_response);
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> PostPublisher([FromBody] PublisherCreateDTO publisherDto)
        {
            var publisher = _mapper.Map<Publisher>(publisherDto);
            await _db.CreateAsync(publisher);
            var publisherObj = _mapper.Map<PublisherCreateDTO>(publisher);

            
            _response.Result = publisherObj;


            return CreatedAtAction(nameof(GetPublishers), new { name = publisherDto.PublisherName }, _response);
        }

    } 
}
