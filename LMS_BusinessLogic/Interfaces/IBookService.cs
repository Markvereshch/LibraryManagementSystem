using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;

namespace LMS_BusinessLogic.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(int? year,
        string? title, string? author, string? genre, BookStatus? status,
        int? collectionId);
        Task<Book> GetBookAsync(int id);
        Task<Book> CreateBookAsync(Book book);
    }
}
