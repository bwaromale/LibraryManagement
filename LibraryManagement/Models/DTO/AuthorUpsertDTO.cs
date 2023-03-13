using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class AuthorUpsertDTO
    {
        [Required]
        public string AuthorName { get; set; }
        [Required]
        public int PublisherId { get; set; }
    }
}
