using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class User 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedTime { get; set; }
        [Required]
        public bool Status { get; set; } = true;
    }
}
