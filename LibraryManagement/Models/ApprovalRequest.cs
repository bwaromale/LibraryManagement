using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class ApprovalRequest
    {
        [Key]
        public int ApprovalId { get; set; }
        public int BorrowingId { get; set; }
        
        public string Role { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool IsApproved { get; set; }

        public virtual Borrowing Borrowing { get; set; }
       
    }
}
