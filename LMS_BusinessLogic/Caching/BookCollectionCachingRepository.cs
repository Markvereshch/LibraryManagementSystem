using LMS_BusinessLogic.Interfaces;
using LMS_DataAccess.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace LMS_DataAccess.Repositories
{
    public class BookCollectionCachingRepository : ICaching<BookCollection>
    {
        private const string CollectionCachePrefix = "BookCollection_";
        private const int Timespan = 2;
        private readonly IDistributedCache _cache;
        public BookCollectionCachingRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task InvalidateCachedCollectionAsync()
        {
            await _cache.RemoveAsync(CollectionCachePrefix + "SET");
        }
        public async Task InvalidateCacheAsync(int collectionId)
        {
            await _cache.RemoveAsync(CollectionCachePrefix + collectionId);
        }
        public async Task<BookCollection?> GetCacheAsync(int collectionId)
        {
            var cachedCollection = await _cache.GetAsync(CollectionCachePrefix + collectionId);
            if (cachedCollection != null)
            {
                var collection = JsonConvert.DeserializeObject<BookCollection>(Encoding.UTF8.GetString(cachedCollection));
                return collection;
            }
            return null;
        }
        public async Task<IEnumerable<BookCollection>?> GetCachedCollectionAsync()
        {
            var cachedCollections = await _cache.GetAsync(CollectionCachePrefix + "SET");
            if (cachedCollections != null)
            {
                var collections = JsonConvert.DeserializeObject<IEnumerable<BookCollection>>(Encoding.UTF8.GetString(cachedCollections));
                return collections;
            }
            return null;
        }
        public async Task SetCacheAsync(BookCollection collection)
        {
            if (collection != null)
            {
                var serializedCollection = JsonConvert.SerializeObject(collection);
                var encodedCollection = Encoding.UTF8.GetBytes(serializedCollection);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Timespan)
                };
                await _cache.SetAsync(CollectionCachePrefix + collection.Id, encodedCollection, options);
            }
        }
        public async Task SetCachedCollectionAsync(IEnumerable<BookCollection> collections)
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
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Timespan)
                };
                await _cache.SetAsync(CollectionCachePrefix + "SET", encodedCollections, options);
            }
        }
    }
}
