using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class ForgotPasswordDto
    {
        [Required]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
