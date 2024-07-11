using AutoMapper;
using LibraryManagementSystem.DTOs;
using LMS_BusinessLogic.Models;

namespace LibraryManagementSystem
{
    public class PresentationLayerMapper : Profile
    {
        public PresentationLayerMapper()
        {
            CreateMap<BookModel, BookDTO>().ReverseMap();
            CreateMap<BookModel, BookOperationsDTO>().ReverseMap();

            CreateMap<BookCollectionModel, BookCollectionDTO>().ReverseMap();
            CreateMap<BookCollectionModel, BookCollectionOperationsDTO>().ReverseMap();
        }
    }
}
