using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.DTO
{
    public class BookDTO
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
        public AuthorDTO Author { get; set; }
    }
}