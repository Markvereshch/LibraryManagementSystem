using LMS_BusinessLogic.Interfaces;
using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;

namespace LMS_BusinessLogic.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public async Task<Book> CreateBookAsync(Book book)
        {
            await _bookRepository.CreateBookAsync(book);
            return book;
        }
        public async Task<IEnumerable<Book>> GetAllBooksAsync(int? year,
        string? title, string? author, string? genre, BookStatus? status,
        int? collectionId)
        {
            var books = await _bookRepository.GetAllBooksAsync();
            if (year >= 0)
            {
                books = books.Where(b => b.Year == year);
            }
            if (title != null)
            {
                books = books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }
            if (author != null)
            {
                books = books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
            }
            if (genre != null)
            {
                books = books.Where(b => b.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));
            }
            if (status != null)
            {
                books = books.Where(b => b.Status == status);
            }
            if (collectionId != null)
            {
                books = books.Where(b => b.CollectionId == collectionId);
            }
            return books;
        }
        public async Task<Book> GetBookAsync(int id)
        {
            return await _bookRepository.GetBookAsync(b => b.Id == id, false);
        }
    }
}
