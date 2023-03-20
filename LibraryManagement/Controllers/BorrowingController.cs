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
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingController : ControllerBase
    {
        
        private readonly IBorrowBook _service;
        private readonly IUser _userServ;
        private readonly IRepository<Book> _bookServ;
        protected APIResponse _response;

        public BorrowingController(IBorrowBook service, IUser user, IRepository<Book> bookServ)
        {
            _service  = service;
            _userServ = user;
            _bookServ = bookServ;
            this._response = new APIResponse();
        }

        // GET: api/Borrowing
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetBorrowings()
        {
            try 
            {
                
                var borrowed = await _service.GetAllAsync();
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = borrowed;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message.ToString());
                return BadRequest(_response);
            }
             
        }

        // GET: api/Borrowing/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Borrowing>> GetBorrowing(int id)
        {
            try
            {
                var borrowing = await _service.GetAsync(b => b.BorrowingId == id);

                if (borrowing == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add($"'{id}' not found");
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = borrowing;

                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message.ToString());
                return BadRequest(_response);
            }
        }
        
       [HttpPost("borrow-book")]
        public async Task<ActionResult<APIResponse>> BorrowRequest(BorrowingDto  borrowingDto)
        {
            try
            {
                if (borrowingDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid input");
                    return BadRequest(_response);
                }
                var userExist = await _userServ.GetAsync(b => b.UserId == borrowingDto.UserId);
                if (userExist == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Invalid User Id");
                    return NotFound(_response);
                }

                var bookExist = await _bookServ.GetAsync(b=>b.BookId == borrowingDto.BookId);
                if (bookExist == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Invalid Book Id");
                    return NotFound(_response);
                }

                var requestExist = await _service.GetAsync(r=>r.UserId == borrowingDto.UserId && r.BookId == borrowingDto.BookId);
                
                if(requestExist.Status == "Pending")
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("You submitted a similar pending request awaiting approval");
                    return BadRequest(_response);
                }
                
                Borrowing borrowBook = new Borrowing()
                {
                    UserId = borrowingDto.UserId,
                    BookId = borrowingDto.BookId,
                    BorrowDate = DateTime.Now,
                    Status = "Pending"
                };

                await _service.CreateAsync(borrowBook);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Borrow Request Sent. Approval is underway";

                return Ok(_response);           
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message.ToString());
                return BadRequest(_response);
            }
        }
        
        // DELETE: api/Borrowing/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowing(int id)
        {
            var borrowing = await _service.GetAsync(b => b.BorrowingId == id);
            if (borrowing == null)
            {
                return NotFound();
            }

           //var deleteBorrowing = await _service.RemoveAsync(borrowing);
            

            return NoContent();
        }

        //private bool BorrowingExists(int id)
        //{
        //    return _context.Borrowing.Any(e => e.BorrowingId == id);
        //}
    }
}
