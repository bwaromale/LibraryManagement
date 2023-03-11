using LibraryManagement.Data;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        private readonly LibraryContext _db;

        public AuthorRepository(LibraryContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Book>> GetBooksAttachedtoAuthor(int id)
        {
            var books = _db.Books.Where(b => b.AuthorId == id).ToListAsync();
            return await books;
        }


    }
}
