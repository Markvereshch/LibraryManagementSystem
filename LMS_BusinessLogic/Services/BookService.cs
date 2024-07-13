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
        private readonly ICaching<Book> _cache;
        public BookService(IBookRepository bookRepository, IMapper mapper, ICaching<Book> cache)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<BookModel> CreateAsync(BookModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            var createdBook = await _bookRepository.CreateAsync(book);
            await _cache.InvalidateCachedCollectionAsync();
            return _mapper.Map<BookModel>(createdBook);
        }
        public async Task DeleteAsync(BookModel bookModel)
        {
            var book = _mapper.Map<Book>(bookModel);
            await _bookRepository.DeleteAsync(book);
            await _cache.InvalidateCacheAsync(book.Id);
            await _cache.InvalidateCachedCollectionAsync();
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
        public async Task<BookModel> GetAsync(int id)
        {
            var book = await _cache.GetCacheAsync(id);
            if (book == null)
            {
                book = await _bookRepository.GetAsync(id);
                await _cache.SetCacheAsync(book);
            }
            return _mapper.Map<BookModel>(book);
        }
        public async Task<BookModel> UpdateAsync(BookModel book, int id)
        {
            var bookToUpdate = await _cache.GetCacheAsync(id);
            if (bookToUpdate == null)
            {
                bookToUpdate = await _bookRepository.GetAsync(id);
            }
            _mapper.Map(book, bookToUpdate);
            bookToUpdate.Id = id;

            var updatedBook = await _bookRepository.UpdateAsync(bookToUpdate);

            await _cache.InvalidateCacheAsync(bookToUpdate.Id);
            await _cache.InvalidateCachedCollectionAsync();
            return _mapper.Map<BookModel>(updatedBook);
        }
        public async Task<(BookModel?, ValidationResults, string)> ValidateExistingModel(int id)
        {
            if (id <= 0)
            {
                return (null, ValidationResults.BadRequest, $"Invalid book ID ({id} is less than 1)");
            }
            var book = await GetAsync(id);
            if (book == null)
            {
                return (null, ValidationResults.NotFound, $"Book with ID={id} cannot be found");
            }
            var bookModel = _mapper.Map<BookModel>(book);
            return (bookModel, ValidationResults.OK, "OK");
        }
    }
}
