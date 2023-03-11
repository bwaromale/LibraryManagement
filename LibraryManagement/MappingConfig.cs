using AutoMapper;
using LibraryManagement.Models;
using LibraryManagement.Models.DTO;

namespace LibraryManagement
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Publisher, PublisherCreateDTO>().ReverseMap();
            CreateMap<Publisher, PublisherDTO>().ReverseMap();  
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Book, BookCreateDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
        }
    }
}
