using LibraryManagement.Models.DTO;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IPublisherRepository : IRepository<Publisher>
    {
        Task<IEnumerable<Author>> GetAuthorsAttachedtoPublisher(int id);
    }
}
