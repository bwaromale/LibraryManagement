using LibraryManagement.Data;
using LibraryManagement.Models.Repository.Interfaces;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class BookRepository : Repository<Book>
    {
        private readonly LibraryContext _db;
        public BookRepository(LibraryContext db) : base(db)
        {
            _db = db;
        }
    }
}
