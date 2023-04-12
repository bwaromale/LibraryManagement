using AutoMapper;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;

namespace LibraryManagement
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Publisher, PublisherUpsertDTO>().ReverseMap();
            CreateMap<Publisher, PublisherDTO>().ReverseMap();  
            CreateMap<Author, AuthorUpsertDTO>().ReverseMap();
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, AuthorWithBooksDTO>().ReverseMap();
            CreateMap<Book, BookUpsertDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, BookWithAuthorDTO>().ReverseMap();
            CreateMap<User, RegisterDTO>().ReverseMap();
            CreateMap<User, UserResponseDto>().ReverseMap();
            CreateMap<Borrowing, ApprovalResponseDto>().ReverseMap();
        }
    }
}
