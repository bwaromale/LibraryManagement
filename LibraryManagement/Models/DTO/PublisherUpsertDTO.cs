using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class PublisherUpsertDTO
    {
        [Required]
        public string PublisherName { get; set; }
        [Required]
        public string Address { get; set; }
        
    }
}
