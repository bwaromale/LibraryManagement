using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        [ForeignKey("Publisher")]
        public int PublisherId { get; set; }
        public DateTime CreatedTime { get; set; }= DateTime.Now;
        public DateTime UpdatedTime { get; set; }
        public Publisher Publishers { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
