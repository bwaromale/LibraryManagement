using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Book>> GetBooksAttachedtoAuthor(int id);
    }
}
