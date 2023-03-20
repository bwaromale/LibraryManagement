using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class ApprovalDto
    {
        [Required]
        public int BorrowingId { get; set; }
        [Required]
        public int ApproverId { get; set; }
    }
}
