using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace LMS_DataAccess.Repositories
{
    public class BookCachingRepository : IBookCaching
    {
        private const string BookCachePrefix = "Book_";
        private const int timespan = 2;
        private readonly IDistributedCache _cache;
        public BookCachingRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public Task DeleteBookAsync(int bookId)
        {
            _cache.Remove(BookCachePrefix + bookId);
            _cache.Remove(BookCachePrefix + "SET");
            return Task.CompletedTask;
        }
        public async Task<Book?> GetBookAsync(int bookId)
        {
            var cachedBook = await _cache.GetAsync(BookCachePrefix + bookId);
            if (cachedBook != null)
            {
                var book = JsonConvert.DeserializeObject<Book>(Encoding.UTF8.GetString(cachedBook));
                return book;
            }
            return null;
        }
        public async Task<IEnumerable<Book>?> GetBooksAsync()
        {
            string cacheKey = BookCachePrefix + "SET";
            var cachedBooks = await _cache.GetAsync(cacheKey);
            if (cachedBooks != null)
            {
                var books = JsonConvert.DeserializeObject<IEnumerable<Book>>(Encoding.UTF8.GetString(cachedBooks));
                return books;
            }
            return null;
        }
        public async Task SetBookAsync(Book book)
        {
            if (book != null)
            {
                var serializedBook = JsonConvert.SerializeObject(book);
                var encodedBook = Encoding.UTF8.GetBytes(serializedBook);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timespan)
                };
                await _cache.SetAsync(BookCachePrefix + book.Id, encodedBook, options);
            }
        }
        public async Task SetBooksAsync(IEnumerable<Book> books)
        {
            if (books != null)
            {
                string cacheKey = BookCachePrefix + "SET";
                var serializedBooks = JsonConvert.SerializeObject(books);
                var encodedBooks = Encoding.UTF8.GetBytes(serializedBooks);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timespan)
                };
                await _cache.SetAsync(cacheKey, encodedBooks, options);
            }
        }
    }
}
