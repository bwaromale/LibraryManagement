using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Role { get; set; } = "User";
    }
}
