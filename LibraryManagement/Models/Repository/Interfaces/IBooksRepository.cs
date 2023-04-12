using LibraryManagement.Models.DTO;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IBooksRepository: IRepository<Book>
    {
        Task<List<BookDTO>> GetBooksAttachedtoAuthor(int id);
    }
}
