using LMS_DataAccess.Entities;
using System.Linq.Expressions;

namespace LMS_DataAccess.Interfaces
{
    public interface IBookCollectionRepository
    {
        Task<IEnumerable<BookCollection>> GetAllCollectionsAsync();
        Task<BookCollection> GetBookCollectionAsync(Expression<Func<BookCollection, bool>> filter, bool isTrackable = false);
        Task<BookCollection> CreateBookCollectionAsync(BookCollection bookCollection);
        Task DeleteBookCollectionAsync(BookCollection bookCollection);
    }
}
