using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using System.Linq.Expressions;

namespace LMS_DataAccess.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookAsync(Expression<Func<Book, bool>> filter = null, bool isTrackable = false);
        Task CreateBookAsync(Book book);
    }
}
