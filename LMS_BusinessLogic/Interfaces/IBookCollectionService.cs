using LMS_DataAccess.Entities;

namespace LMS_BusinessLogic.Interfaces
{
    public interface IBookCollectionService
    {
        Task<IEnumerable<BookCollection>> GetAllCollectionsAsync();
    }
}
