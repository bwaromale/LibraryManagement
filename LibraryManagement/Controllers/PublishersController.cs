using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherRepository _publisherServ;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public PublishersController(IPublisherRepository publisherServ, IMapper mapper)
        {
            _publisherServ = publisherServ;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet]
        [Authorize(Roles ="Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetPublishers()
        {
            try
            {
                IEnumerable<Publisher> publishers = await _publisherServ.GetAllAsync();
                if (!publishers.Any())
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Publishers records is empty");
                    return NotFound(_response);
                }
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
        [HttpGet("{publisherName}")]
        [Authorize(Roles = "Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetPublisher(string publisherName)
        {
            try
            {
                var publisher = await _publisherServ.GetAsync(p=>p.PublisherName ==publisherName);
                if(publisher == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add($"'{publisherName}' not found");
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<PublisherUpsertDTO>(publisher);
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
        [Authorize(Roles = "Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetAuthorsAttachedtoPublisher(int id)
        {
            try
            {
                var authors = await _publisherServ.GetAuthorsAttachedtoPublisher(id);
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResponse>> PostPublisher([FromBody] PublisherUpsertDTO publisherDto)
        {
            try
            {
                var publisher = _mapper.Map<Publisher>(publisherDto);
                var publisherExist = await _publisherServ.CheckDuplicateAtCreation(p => p.PublisherName == publisherDto.PublisherName);
                if (publisherExist)
                {
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>() { $"A publisher with name '{publisher.PublisherName}' already exist." };
                    return BadRequest(_response);
                }
                await _publisherServ.CreateAsync(publisher);

                _response.Result = _mapper.Map<PublisherUpsertDTO>(publisher);
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
        [HttpDelete("{publisherName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResponse>> DeletePublisher(string publisherName)
        {
            try
            {
                 
                if (string.IsNullOrEmpty(publisherName))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Invalid input"};
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var publisher = await _publisherServ.GetAsync(p=>p.PublisherName == publisherName);
                if(publisher == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() {$"'{publisherName}' not found" };
                    return BadRequest(_response);
                }
                await _publisherServ.RemoveAsync(p => p.PublisherName ==publisherName);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }
        [HttpPut("{publisherName}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<APIResponse>> UpdatePublisher(string publisherName, [FromBody] PublisherUpsertDTO publisherDTO)
        {
            try
            {
                if(publisherDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string>() { "Invalid input"};
                    return BadRequest(_response);
                }
                var publisher = await _publisherServ.GetAsync(p => p.PublisherName == publisherName);
                if (publisher == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() { $"Publisher with name '{publisherName}' not found" };
                    return NotFound(_response);
                }
                
                publisher.PublisherName = publisherDTO.PublisherName;
                publisher.Address = publisherDTO.Address;
                publisher.UpdatedDate = DateTime.Now;
                await _publisherServ.UpdateAsync(publisher);

                _response.StatusCode=HttpStatusCode.OK;
                _response.Result = publisherDTO;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages=new List<string>() { ex.ToString() };
                return BadRequest(_response);
            }
        }

    } 
}
