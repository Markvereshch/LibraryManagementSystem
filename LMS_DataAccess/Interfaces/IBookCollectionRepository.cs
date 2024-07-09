using LMS_DataAccess.Entities;

namespace LMS_DataAccess.Interfaces
{
    public interface IBookCollectionRepository
    {
        Task<IEnumerable<BookCollection>> GetAllCollectionsAsync();
    }
}
