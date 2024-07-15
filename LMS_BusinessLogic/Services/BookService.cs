using AutoMapper;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Models;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using LMS_Shared;

namespace LMS_BusinessLogic.Services
{
    public class BookService : Service<BookModel, Book>, IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository, IMapper mapper, ICaching<Book> cache) : base(bookRepository, mapper, cache)
        {
            _bookRepository = bookRepository;
        }
        public async Task<IEnumerable<BookModel>> GetAllAsync(BookFiltersModel filters)
        {
            var books = await _cache.GetCachedCollectionAsync();
            if (books == null)
            {
                books = await _bookRepository.GetAllAsync(filters);
                await _cache.SetCachedCollectionAsync(books);
            }
            var booksModel = _mapper.Map<List<BookModel>>(books);
            return booksModel;
        }
        public async Task<IEnumerable<BookModel>> GetAllCopiesAsync(BookModel model)
        {
            var books = await _bookRepository.GetAllAsync(new BookFiltersModel());
            var copies = books.Where(c => string.Equals(c.Author, model.Author, StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(c.Title, model.Title, StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(c.Genre, model.Genre, StringComparison.OrdinalIgnoreCase)
                                    && c.Year == model.Year);
            return _mapper.Map<List<BookModel>>(copies);
        }
    }
}
