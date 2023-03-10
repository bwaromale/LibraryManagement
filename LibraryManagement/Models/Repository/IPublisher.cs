using LibraryManagement.Models.DTO;

namespace LibraryManagement.Models.Repository
{
    public interface IPublisher:IRepository<Publisher>
    {
        Task<IEnumerable<Author>> GetAuthorsAttachedtoPublisher(int id);
    }
}
