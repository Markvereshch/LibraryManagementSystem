using LMS_BusinessLogic.Models;

namespace LMS_BusinessLogic.Interfaces
{
    public interface IBookCollectionService
    {
        Task<IEnumerable<BookCollectionModel>> GetAllCollectionsAsync();
        Task<BookCollectionModel> GetBookCollectionAsync(int id);
        Task<BookCollectionModel> CreateBookCollectionAsync(BookCollectionModel collectionModel);
        Task DeleteBookCollectionAsync(BookCollectionModel collectionModel);
    }
}
