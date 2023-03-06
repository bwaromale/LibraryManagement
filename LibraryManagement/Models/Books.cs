using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

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
        public DateTime CreatedDate { get; set; }
        [Required]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }
    }
}
