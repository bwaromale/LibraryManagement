using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models.DTO
{
    public class AuthorDTO
    {
        [Required]
        public string AuthorName { get; set; }
       
    }
}
