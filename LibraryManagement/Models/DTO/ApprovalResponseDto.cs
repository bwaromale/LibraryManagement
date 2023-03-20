namespace LibraryManagement.Models.DTO
{
    public class ApprovalResponseDto
    {
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Message { get; set; } = "Borrowing Successfully Approved";
       
    }
}
