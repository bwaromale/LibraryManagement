using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Models.DTO;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        private readonly LibraryContext _db;
        private readonly IMapper _mapper;

        public AuthorRepository(LibraryContext db, IMapper mapper) : base(db)
        {
            _db = db;
            _mapper = mapper;
        }

       


    }
}
