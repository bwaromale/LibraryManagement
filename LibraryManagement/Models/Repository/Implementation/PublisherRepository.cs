using LibraryManagement.Data;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class PublisherRepository : Repository<Publisher>, IPublisherRepository
    {
        private readonly LibraryContext _db;
        public PublisherRepository(LibraryContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Author>> GetAuthorsAttachedtoPublisher(int id)
        {
            var authors = _db.Authors.Where(p => p.PublisherId == id).ToListAsync();
            return await authors;
        }
        
    }
}
