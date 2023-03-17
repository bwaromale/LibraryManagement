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
            CreateMap<Book, BookUpsertDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<User, RegisterDTO>().ReverseMap();
        }
    }
}
