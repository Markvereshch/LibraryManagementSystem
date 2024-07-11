using LMS_DataAccess.Entities;

namespace LMS_DataAccess.Interfaces
{
    public interface IBookCollectionCaching
    {
        Task<BookCollection?> GetBookCollectionAsync(int collectionId);
        Task SetBookCollectionAsync(BookCollection collection);
        Task<IEnumerable<BookCollection>?> GetBookCollectionsAsync();
        Task SetBookCollectionsAsync(IEnumerable<BookCollection> collections);
        Task DeleteCollectionAsync(int collectionId);
    }
}
