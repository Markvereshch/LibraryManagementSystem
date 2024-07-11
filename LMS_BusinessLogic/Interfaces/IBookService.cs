using LMS_BusinessLogic.Models;
using LMS_Shared;

namespace LMS_BusinessLogic.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookModel>> GetAllBooksAsync(int? year = null,
        string? title = null, string? author = null, string? genre = null, BookStatus? status = null,
        int? collectionId = null);
        Task<BookModel> GetBookAsync(int id);
        Task<BookModel> CreateBookAsync(BookModel book);
        Task DeleteBookAsync(BookModel book);
        Task<BookModel> UpdateBookAsync(BookModel book, int id);
    }
}
