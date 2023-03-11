using AutoMapper;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherRepository _db;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public PublishersController(IPublisherRepository db, IMapper mapper)
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
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            } 
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetPublisher(int id)
        {
            try
            {
                var publisher = await _db.GetAsync(id);
                _response.Result = _mapper.Map<PublisherDTO>(publisher);
                _response.IsSuccess = true;
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
        [HttpGet("{id}/authors")]
        public async Task<ActionResult<APIResponse>> GetAuthorsAttachedtoPublisher(int id)
        {
            try
            {
                var authors = await _db.GetAuthorsAttachedtoPublisher(id);
                if (!authors.Any())
                {
                    _response.ErrorMessages = new List<string>() { "Not Found" };
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<IEnumerable<AuthorDTO>>(authors);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> PostPublisher([FromBody] PublisherCreateDTO publisherDto)
        {
            try
            {
                var publisher = _mapper.Map<Publisher>(publisherDto);
                var publisherExist = await _db.CheckDuplicateAtCreation(p => p.PublisherName == publisherDto.PublisherName);
                if (publisherExist)
                {
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { $"A publisher with name '{publisher.PublisherName}' already exist." };
                    return BadRequest(_response);
                }
                await _db.CreateAsync(publisher);

                _response.Result = _mapper.Map<PublisherCreateDTO>(publisher);
                _response.StatusCode = HttpStatusCode.Created;


                return CreatedAtAction(nameof(GetPublishers), new { name = publisherDto.PublisherName }, _response);
            }
            catch(Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }

    } 
}
