using AutoMapper;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingController : ControllerBase
    {
        
        private readonly IBorrowBook _borrowServ;
        private readonly IUser _userServ;
        private readonly IBooksRepository _bookServ;
        private readonly IEmail _emailServ;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public BorrowingController(IBorrowBook borrowServ, IUser user, IBooksRepository bookServ, IEmail emailServ, IMapper mapper)
        {
            _borrowServ  = borrowServ;
            _userServ = user;
            _bookServ = bookServ;
            _emailServ = emailServ;
            _mapper = mapper;
            this._response = new APIResponse();
        }

        [HttpGet]
        [Authorize(Roles ="Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetBorrowings()
        {
            try 
            {
                
                var borrowed = await _borrowServ.GetAllAsync();
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
        [Authorize(Roles = "Admin, Approver")]
        public async Task<ActionResult<Borrowing>> GetBorrowing(int id)
        {
            try
            {
                
                var borrowing = await _borrowServ.GetAsync(b => b.BorrowingId == id);
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
        [HttpGet("{userId}/borrowings")]
        [Authorize(Roles = "Admin, Approver")]
        public async Task<ActionResult<APIResponse>> GetBorrowingsAttachedToUser(int userId)
        {
            try
            {
                if(userId <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid Input");
                    return BadRequest(_response);
                }

                IEnumerable<Borrowing> borrowings = await _borrowServ.GetAllBorrowingWithConditionAsync(u => u.UserId == userId);
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = borrowings;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess=false;
                _response.StatusCode = HttpStatusCode.BadGateway;
                _response.ErrorMessages.Add(ex.ToString());
                return BadRequest(_response);
            }
        }
        
        [HttpPost("borrow-book")]
        [Authorize(Roles ="User")]
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
                //confirm if authenticated userid and userid in request match
                int userId;
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId) || userId != borrowingDto.UserId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.ErrorMessages.Add("Unauthorized: You cannot make a request for another user");
                    return Unauthorized(_response);
                }

                borrowingDto.UserId = userId;
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

                if (bookExist.TotalCopies == 0 || bookExist.Available == false || bookExist.BorrowedCopies >= bookExist.TotalCopies)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add($"'{bookExist.Title}' is not available at the moment. Try again later");
                    return BadRequest(_response);
                }

                var requestExist = await _borrowServ.GetAllBorrowingWithConditionAsync(r=>r.UserId == borrowingDto.UserId && r.BookId == borrowingDto.BookId);
                
                if(requestExist != null)
                {
                    if (requestExist.Any(r => r.Status == "Pending"))
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("You submitted a similar pending request awaiting approval");
                        return BadRequest(_response);
                    }
                }
                //check if the user has a book he has not returned on due date
                var pendingCheckIn = await _borrowServ.GetAllBorrowingWithConditionAsync(p=>p.UserId == borrowingDto.UserId && p.ReturnDate < DateTime.Now && p.Status == "Approved" && p.CheckIn == false );
                if(pendingCheckIn != null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.ErrorMessages.Add("Pending Checkin. Borrower must clear outstanding checkin");
                    return Unauthorized(_response);
                }
                Borrowing borrowBook = new Borrowing()
                {
                    UserId = borrowingDto.UserId,
                    BookId = borrowingDto.BookId,
                    BorrowDate = DateTime.Now,
                    Status = "Pending"
                };

                 
                await _borrowServ.CreateAsync(borrowBook);
                

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Borrow Request Sent. Approval is underway";

                return Ok(_response);           
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.ToString());
                return BadRequest(_response);
            }
        }
        
        // DELETE: api/Borrowing/5
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteBorrowing(int id)
        {
            var borrowing = await _borrowServ.GetAsync(b => b.BorrowingId == id);
            if (borrowing == null)
            {
                return NotFound();
            }

            await _borrowServ.RemoveAsync(d=>d.BookId == id);
            

            return NoContent();
        }

        [HttpPut("approve-borrowing")]
        [Authorize(Roles ="Approver")]
        public async Task<ActionResult<APIResponse>> ApproveBorrowing(ApprovalDto approvalDto)
        {
            if(approvalDto == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Invalid input");
                return BadRequest(_response);
            }

            var request = await _borrowServ.GetAsync(r=>r.BorrowingId ==approvalDto.BorrowingId);
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
                _response.ErrorMessages.Add("Invalid approver id");
                return NotFound(_response);
            }
            if(approver.Role != "Approver")
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("You are not an approver");
                return NotFound(_response);
            }

            var borrower = await _userServ.GetAsync(b => b.UserId == request.UserId);
            if (borrower == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Borrower details not found");
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            var book = await _bookServ.GetAsync(b => b.BookId == request.BookId);
            if (book == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Book details not found");
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            if(book.TotalCopies == 0 || book.Available == false)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add($"'{book.Title}' is not available at the moment. Try again later");
                return BadRequest(_response);
            }

            if (book.BorrowedCopies < book.TotalCopies)
            {
                book.BorrowedCopies++;
                await _bookServ.UpdateAsync(book);
            }
            request.ReturnDate = DateTime.Now.AddDays(3);
            request.IsApproved = true;
            request.ApprovedBy = approvalDto.ApproverId;
            request.ApprovalDate = DateTime.Now;
            request.Status = "Approved";

            await _borrowServ.UpdateAsync(request);
            EmailDto emailDto = new EmailDto()
            {
                From = "SHAWNLIBRARY@GMAIL.COM",
                To = borrower.Email,
                Subject = "BOOK APPROVAL NOTIFICATION",
                Body = $"Hello {borrower.FirstName}, your request to borrow {book.Title} by {book.Author} is approved. Kindly proceed to pick up. "
            };
            _emailServ.SendEmail(emailDto);
            var map = _mapper.Map<ApprovalResponseDto>(request);
            _response.StatusCode=HttpStatusCode.OK;
            _response.Result = map;

            return Ok(_response);
        }
        [HttpPut("revoke-borrowing")]
        [Authorize(Roles = "Approver")]
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

                var request = await _borrowServ.GetBorrowAsync(approvalDto.BorrowingId);
                if (request == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Borrowing Request not Found");
                    return NotFound(_response);
                }

                var approver = await _userServ.GetAsync(a=>a.UserId == approvalDto.ApproverId);
                if (approver == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Approver not found");
                    return NotFound(_response);
                }
                
                if (approver.Role != "Approver")
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.ErrorMessages.Add("You are not an approver");
                    return Unauthorized(_response);
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

                await _borrowServ.UpdateAsync(request);

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
        [HttpPut("check-in")]
        [Authorize(Roles = "Admin, Approver")]
        public async Task<ActionResult<APIResponse>> BookCheckIn(int borrowingId) 
        {
            try
            {
                if (borrowingId <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid input");
                    return BadRequest(_response);
                }

                var borrowing = await _borrowServ.GetAsync(b => b.BorrowingId == borrowingId);
                if (borrowing == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Invalid borrowing ID");
                    return NotFound(_response);
                }

                if (borrowing.CheckOut == false)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Book has not being being Checked Out");
                    return BadRequest(_response);
                }

                if (borrowing.CheckIn == true)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Forbidden;
                    _response.ErrorMessages.Add($"Book already checked in by {borrowing.CheckInDate}.");
                    return BadRequest(_response);
                }
                borrowing.CheckIn = true;
                borrowing.CheckInDate = DateTime.Now;

                await _borrowServ.UpdateAsync(borrowing);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = $"Book Returned and Check In by {DateTime.Now}";
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
        [HttpPut("check-out")]
        [Authorize(Roles = "Admin, Approver")]
        public async Task<ActionResult<APIResponse>> BookCheckOut(int borrowingId)
        {
            try {
                if (borrowingId <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Invalid input");
                    return BadRequest(_response);
                }

                var borrowing = await _borrowServ.GetAsync(b => b.BorrowingId == borrowingId);
                if (borrowing == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Invalid borrowing ID");
                    return NotFound(_response);
                }

                if (borrowing.CheckOut == true)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add($"Book has already being Checked Out at {borrowing.CheckOutDate}");
                    return BadRequest(_response);
                }

                borrowing.CheckOut = true;
                borrowing.CheckOutDate = DateTime.Now;

                await _borrowServ.UpdateAsync(borrowing);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = $"Book Collected and Checked Out by {DateTime.Now}";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message.ToString());
                return BadRequest(_response);
            }
        }
    }
}
