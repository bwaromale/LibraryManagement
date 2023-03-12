using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class PublisherDTO
    {
        [Required]
        public string PublisherName { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
