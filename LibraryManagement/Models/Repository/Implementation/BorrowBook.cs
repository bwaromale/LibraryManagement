﻿using LibraryManagement.Data;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class BorrowBook : Repository<Borrowing>,IBorrowBook
    {
        private readonly LibraryContext _context;
        private readonly IRepository<Borrowing> _repo;

        public BorrowBook(LibraryContext context, IRepository<Borrowing> repo):base(context)
        {
            _context = context;
            _repo = repo;
        }

        public void ApproveRequest(int borrowingId, int attendantId)
        {
            var request = _context.Borrowing.Find(borrowingId);
            if (request == null)
            {
                throw new ArgumentException("Invalid request ID.");
            }

            if (request.Status != "Pending")
            {
                throw new InvalidOperationException("Request is not pending.");
            }

            request.Status = "Approved";
            request.ApprovalDate = DateTime.UtcNow;
            request.UserId = attendantId;

            _context.SaveChanges();
        }
        public void RejectRequest(int requestId, int attendantId, string rejectedReason)
        {
            var request = _context.Borrowing.Include(r => r.Book).SingleOrDefault(r => r.BorrowingId == requestId);
            if (request == null)
            {
                throw new ArgumentException("Invalid request ID.");
            }

            if (request.Status != "Pending")
            {
                throw new InvalidOperationException("Request is not pending.");
            }

            request.Status = "Rejected";
            request.ApprovalDate = DateTime.UtcNow;
            request.UserId = attendantId;

            _context.Update(request);

            var book = request.Book;
            book.TotalCopies++;

            _context.Update(book);

            _context.SaveChanges();
        }
        public void FinalApproveRequest(int requestId, int librarianId)
        {
            var request = _context.Borrowing.Find(requestId);
            if (request == null)
            {
                throw new ArgumentException("Invalid request ID.");
            }

            if (request.Status != "ApprovedByAttendant")
            {
                throw new InvalidOperationException("Request is not approved by attendant.");
            }

            var librarian = _context.Users.Find(librarianId);
            if (librarian == null)
            {
                throw new ArgumentException("Invalid librarian ID.");
            }

            request.Status = "ApprovedByLibrarian";
            request.ApprovedBy = librarian.UserId;
            request.ApprovalDate = DateTime.Now;

            _context.SaveChanges();
        }

    }
}
