using LMS_DataAccess.Entities;

namespace LMS_DataAccess.Interfaces
{
    public interface IBookCaching
    {
        Task<Book?> GetBookAsync(int bookId);
        Task SetBookAsync(Book book);
        Task<IEnumerable<Book>?> GetBooksAsync();
        Task SetBooksAsync(IEnumerable<Book> books);
        Task DeleteBookAsync(int bookId);
    }
}
