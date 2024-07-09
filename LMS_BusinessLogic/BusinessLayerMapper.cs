using AutoMapper;
using LMS_BusinessLogic.Models;
using LMS_DataAccess.Entities;

namespace LMS_BusinessLogic
{
    public class BusinessLayerMapper : Profile
    {
        public BusinessLayerMapper()
        {
            CreateMap<BookModel, Book>().ReverseMap();
            CreateMap<BookCollectionModel, BookCollection>().ReverseMap();
        }
    }
}
