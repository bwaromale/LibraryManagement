﻿using System.Linq.Expressions;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IBorrowBook: IRepository<Borrowing>
    {
            Task<Borrowing> GetBorrowAsync(int BorrowingId);
            Task<IEnumerable<Borrowing>> GetAllBorrowingWithConditionAsync(Func<Borrowing, bool> predicate = null);
            Task<IEnumerable<Borrowing>> GetUserBorrowings(Func<Borrowing, bool> predicate = null);
            void ApproveRequest(int requestId, int attendantId);
            void RejectRequest(int requestId, int attendantId, string rejectedReason);
            void FinalApproveRequest(int requestId, int librarianId);
    }
}
