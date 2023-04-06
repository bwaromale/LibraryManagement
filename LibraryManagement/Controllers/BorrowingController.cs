using AutoMapper;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
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
        private readonly IBorrowBook _borrow;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public BorrowingController(IBorrowBook service, IUser user, IRepository<Book> bookServ, IBorrowBook borrowBook, IMapper mapper)
        {
            _service  = service;
            _userServ = user;
            _bookServ = bookServ;
            _borrow = borrowBook;
            _mapper = mapper;
            this._response = new APIResponse();
        }

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
                
                if(requestExist != null)
                {
                    if (requestExist.Status == "Pending")
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("You submitted a similar pending request awaiting approval");
                        return BadRequest(_response);
                    }
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

        [HttpPut("approve-borrowing")]
        public async Task<ActionResult<APIResponse>> AttendantApproveBorrowing(ApprovalDto approvalDto)
        {
            if(approvalDto == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Invalid input");
                return BadRequest(_response);
            }

            var request = await _service.GetAsync(r=>r.BorrowingId ==approvalDto.BorrowingId);
            if (request == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Borrowing request not found");
                return NotFound(_response);
            }
            var approver = await _userServ.GetAsync(a => a.UserId == approvalDto.ApproverId);
            if(approver == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Invalid Approver Id");
                return NotFound(_response);
            }
            if(approver.Role != "Approver")
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("You are not an approver");
                return NotFound(_response);
            }

            request.ReturnDate = DateTime.Now.AddDays(3);
            request.IsApproved = true;
            request.ApprovedBy = approvalDto.ApproverId;
            request.ApprovalDate = DateTime.Now;
            request.Status = "Approved";

            await _service.UpdateAsync(request);

            var map = _mapper.Map<ApprovalResponseDto>(request);
            _response.StatusCode=HttpStatusCode.OK;
            _response.Result = map;

            return Ok(_response);
        }
        [HttpPut("revoke-borrowing")]
        public async Task<ActionResult<APIResponse>> RevokeBorrowing(ApprovalDto approvalDto)
        {
            try
            {
                if (approvalDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid input");
                    return BadRequest(_response);
                }

                var request = await _borrow.GetBorrowAsync(approvalDto.BorrowingId);
                if (request == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Borrowing Request not Found");
                    return NotFound(_response);
                }
                if(request.IsApproved == false)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Only approved requests can be revoked");
                    return BadRequest(_response);
                }
                request.RevokedDate = DateTime.Now;
                request.Status = "Revoked";
                request.RevokeStatus = true;
                request.RevokedBy = approvalDto.ApproverId;

                await _service.UpdateAsync(request);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Revoke Successful";

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
    }
}
