using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class ResetPasswordDto
    {
        [Required]
        public string Username { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string ResetToken { get; set; }
    }
}
