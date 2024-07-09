using AutoMapper;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Models;
using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using LMS_Shared;

namespace LMS_BusinessLogic.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }
        public async Task<BookModel> CreateBookAsync(BookModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            var createdBook = await _bookRepository.CreateBookAsync(book);
            return _mapper.Map<BookModel>(createdBook);
        }
        public async Task DeleteBookAsync(BookModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            await _bookRepository.DeleteBookAsync(book);
        }
        public async Task<IEnumerable<BookModel>> GetAllBooksAsync(int? year,
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
            var booksModel = _mapper.Map<List<BookModel>>(books);
            return booksModel;
        }
        public async Task<BookModel> GetBookAsync(int id)
        {
            var book = await _bookRepository.GetBookAsync(b => b.Id == id, false);
            return _mapper.Map<BookModel>(book);
        }
        public async Task<BookModel> UpdateBookAsync(BookModel book, int id)
        {
            var bookToUpdate = await _bookRepository.GetBookAsync(b => b.Id == id, false);
            bookToUpdate.Title = book.Title;
            bookToUpdate.Author = book.Author;
            bookToUpdate.Genre = book.Genre;
            bookToUpdate.Year = book.Year;
            bookToUpdate.Status = book.Status;
            bookToUpdate.CollectionId = book.CollectionId;

            var updatedBook = await _bookRepository.UpdateBookAsync(bookToUpdate);
            return _mapper.Map<BookModel>(updatedBook);
        }
    }
}
