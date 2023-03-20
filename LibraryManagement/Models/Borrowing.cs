using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Borrowing
    {
        [Key]
        public int BorrowingId { get; set; }
        [ForeignKey("Users")]
        public int UserId { get; set; }
        [ForeignKey("Books")]
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool IsApproved { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string Status { get; set; }

        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}
