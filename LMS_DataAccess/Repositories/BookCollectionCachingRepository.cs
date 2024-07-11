using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace LMS_DataAccess.Repositories
{
    public class BookCollectionCachingRepository : IBookCollectionCaching
    {
        private const string CollectionCachePrefix = "BookCollection_";
        private const int timespan = 2;
        private readonly IDistributedCache _cache;
        public BookCollectionCachingRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public Task DeleteCollectionAsync(int collectionId)
        {
            _cache.Remove(CollectionCachePrefix + collectionId);
            _cache.Remove(CollectionCachePrefix + "SET");
            return Task.CompletedTask;
        }
        public async Task<BookCollection?> GetBookCollectionAsync(int collectionId)
        {
            var cachedCollection = await _cache.GetAsync(CollectionCachePrefix + collectionId);
            if (cachedCollection != null)
            {
                var collection = JsonConvert.DeserializeObject<BookCollection>(Encoding.UTF8.GetString(cachedCollection));
                return collection;
            }
            return null;
        }
        public async Task<IEnumerable<BookCollection>?> GetBookCollectionsAsync()
        {
            var cachedCollections = await _cache.GetAsync(CollectionCachePrefix + "SET");
            if (cachedCollections != null)
            {
                var collections = JsonConvert.DeserializeObject<IEnumerable<BookCollection>>(Encoding.UTF8.GetString(cachedCollections));
                return collections;
            }
            return null;
        }
        public async Task SetBookCollectionAsync(BookCollection collection)
        {
            if (collection != null)
            {
                var serializedCollection = JsonConvert.SerializeObject(collection);
                var encodedCollection = Encoding.UTF8.GetBytes(serializedCollection);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timespan)
                };
                await _cache.SetAsync(CollectionCachePrefix + collection.Id, encodedCollection, options);
            }
        }
        public async Task SetBookCollectionsAsync(IEnumerable<BookCollection> collections)
        {
            if (collections != null)
            {
                var serializedCollections = JsonConvert.SerializeObject(collections, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
                var encodedCollections = Encoding.UTF8.GetBytes(serializedCollections);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timespan)
                };
                await _cache.SetAsync(CollectionCachePrefix + "SET", encodedCollections, options);
            }
        }
    }
}
