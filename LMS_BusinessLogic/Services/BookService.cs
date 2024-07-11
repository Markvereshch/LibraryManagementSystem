using AutoMapper;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Models;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using LMS_Shared;

namespace LMS_BusinessLogic.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IBookCaching _cache;
        public BookService(IBookRepository bookRepository, IMapper mapper, IBookCaching cache)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<BookModel> CreateBookAsync(BookModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            var createdBook = await _bookRepository.CreateBookAsync(book);
            await _cache.DeleteBookAsync(createdBook.Id);
            return _mapper.Map<BookModel>(createdBook);
        }
        public async Task DeleteBookAsync(BookModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            await _bookRepository.DeleteBookAsync(book);
            await _cache.DeleteBookAsync(book.Id);
        }
        public async Task<IEnumerable<BookModel>> GetAllBooksAsync(int? year,
        string? title, string? author, string? genre, BookStatus? status,
        int? collectionId)
        {
            var books = await _cache.GetBooksAsync();
            if (books == null)
            {
                books = await _bookRepository.GetAllBooksAsync();
                await _cache.SetBooksAsync(books);
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
            }
            var booksModel = _mapper.Map<List<BookModel>>(books);
            return booksModel;
        }
        public async Task<BookModel> GetBookAsync(int id)
        {
            var book = await _cache.GetBookAsync(id);
            if (book == null)
            {
                book = await _bookRepository.GetBookAsync(b => b.Id == id, false);
                await _cache.SetBookAsync(book);
            }
            return _mapper.Map<BookModel>(book);
        }
        public async Task<BookModel> UpdateBookAsync(BookModel book, int id)
        {
            var bookToUpdate = await _cache.GetBookAsync(id);
            if (bookToUpdate == null)
            {
                bookToUpdate = await _bookRepository.GetBookAsync(b => b.Id == id, false);
            }
            bookToUpdate.Title = book.Title;
            bookToUpdate.Author = book.Author;
            bookToUpdate.Genre = book.Genre;
            bookToUpdate.Year = book.Year;
            bookToUpdate.Status = book.Status;
            bookToUpdate.CollectionId = book.CollectionId;

            var updatedBook = await _bookRepository.UpdateBookAsync(bookToUpdate);
            await _cache.DeleteBookAsync(bookToUpdate.Id);
            await _cache.SetBookAsync(bookToUpdate);
            return _mapper.Map<BookModel>(updatedBook);
        }
    }
}
