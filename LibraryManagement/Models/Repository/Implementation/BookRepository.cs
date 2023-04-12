using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class BookRepository : Repository<Book>, IBooksRepository
    {
        private readonly LibraryContext _db;
        private readonly IMapper _mapper;

        public BookRepository(LibraryContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<List<BookDTO>> GetBooksAttachedtoAuthor(int id)
        {
            var books = await _db.Books.Where(b => b.AuthorId == id).ToListAsync();
            var mapBooks = _mapper.Map<List<BookDTO>>(books);
            return mapBooks;
        }
    }
}
