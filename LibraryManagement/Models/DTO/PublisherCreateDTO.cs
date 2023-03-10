using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class PublisherCreateDTO
    {
        [Required]
        public string PublisherName { get; set; }
        [Required]
        public string Address { get; set; }
        
    }
}
