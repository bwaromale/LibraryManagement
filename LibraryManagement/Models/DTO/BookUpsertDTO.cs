using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class BookUpsertDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string CoverType { get; set; }
        [Required]
        public string NoOfPages { get; set; }
        [Required]
        public string ForewardBy { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public bool Available { get; set; }
        
        [Required]
        public int AuthorId { get; set; }

    }
}
