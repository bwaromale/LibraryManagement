﻿namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IBorrowBook
    {
            void ApproveRequest(int requestId, int attendantId);
            void RejectRequest(int requestId, int attendantId, string rejectedReason);
            void FinalApproveRequest(int requestId, int librarianId);
    }
}