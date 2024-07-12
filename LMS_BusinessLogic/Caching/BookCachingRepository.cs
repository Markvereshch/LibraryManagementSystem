using LMS_BusinessLogic.Interfaces;
using LMS_DataAccess.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace LMS_DataAccess.Repositories
{
    public class BookCachingRepository : ICaching<Book>
    {
        private const string BookCachePrefix = "Book_";
        private const int Timespan = 2;
        private readonly IDistributedCache _cache;
        public BookCachingRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task InvalidateCachedCollectionAsync()
        {
            await _cache.RemoveAsync(BookCachePrefix + "SET");
        }
        public async Task InvalidateCacheAsync(int bookId)
        {
            await _cache.RemoveAsync(BookCachePrefix + bookId);
        }
        public async Task<Book?> GetCacheAsync(int bookId)
        {
            var cachedBook = await _cache.GetAsync(BookCachePrefix + bookId);
            if (cachedBook != null)
            {
                var book = JsonConvert.DeserializeObject<Book>(Encoding.UTF8.GetString(cachedBook));
                return book;
            }
            return null;
        }
        public async Task<IEnumerable<Book>?> GetCachedCollectionAsync()
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
        public async Task SetCacheAsync(Book book)
        {
            if (book != null)
            {
                var serializedBook = JsonConvert.SerializeObject(book);
                var encodedBook = Encoding.UTF8.GetBytes(serializedBook);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Timespan)
                };
                await _cache.SetAsync(BookCachePrefix + book.Id, encodedBook, options);
            }
        }
        public async Task SetCachedCollectionAsync(IEnumerable<Book> books)
        {
            if (books != null)
            {
                string cacheKey = BookCachePrefix + "SET";
                var serializedBooks = JsonConvert.SerializeObject(books);
                var encodedBooks = Encoding.UTF8.GetBytes(serializedBooks);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Timespan)
                };
                await _cache.SetAsync(cacheKey, encodedBooks, options);
            }
        }
    }
}
