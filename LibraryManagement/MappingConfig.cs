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
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            CreateMap<Book, BookCreateDTO>().ReverseMap();
        }
    }
}
