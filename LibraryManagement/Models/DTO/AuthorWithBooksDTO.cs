namespace LibraryManagement.Models.DTO
{
    public class AuthorWithBooksDTO
    {
        public string AuthorName { get; set; }
        public List<BookDTO> Books { get; set; }
    }
}
