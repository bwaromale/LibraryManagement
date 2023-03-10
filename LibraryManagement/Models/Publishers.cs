
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryManagement.Models
{
    public class Publisher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.Now;
        public virtual ICollection<Author> Authors { get; set; }

    }
}
