using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BorrowingController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Borrowing
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Borrowing>>> GetBorrowings()
        {
            return await _context.Borrowing.ToListAsync();
        }

        // GET: api/Borrowing/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Borrowing>> GetBorrowing(int id)
        {
            var borrowing = await _context.Borrowing.FindAsync(id);

            if (borrowing == null)
            {
                return NotFound();
            }

            return borrowing;
        }

        // PUT: api/Borrowing/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBorrowing(int id, Borrowing borrowing)
        {
            if (id != borrowing.BorrowingId)
            {
                return BadRequest();
            }

            _context.Entry(borrowing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Borrowing
        [HttpPost]
        public async Task<ActionResult<Borrowing>> PostBorrowing(Borrowing borrowing)
        {
            _context.Borrowing.Add(borrowing);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBorrowing), new { id = borrowing.BorrowingId }, borrowing);
        }

        // DELETE: api/Borrowing/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowing(int id)
        {
            var borrowing = await _context.Borrowing.FindAsync(id);
            if (borrowing == null)
            {
                return NotFound();
            }

            _context.Borrowing.Remove(borrowing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BorrowingExists(int id)
        {
            return _context.Borrowing.Any(e => e.BorrowingId == id);
        }
    }
}
