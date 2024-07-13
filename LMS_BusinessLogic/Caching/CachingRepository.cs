using LMS_BusinessLogic.Interfaces;
using LMS_DataAccess.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace LMS_BusinessLogic.Caching
{
    public class CachingRepository<T> : ICaching<T> where T : class, IEntity
    {
        private readonly string _cachePrefix;
        private readonly int _timespan;
        private readonly IDistributedCache _cache;
        public CachingRepository(IDistributedCache cache, string cachePrefix, int timespan)
        {
            _cache = cache;
            _cachePrefix = cachePrefix;
            _timespan = timespan;
        }
        public async Task InvalidateCachedCollectionAsync()
        {
            await _cache.RemoveAsync(_cachePrefix + "SET");
        }
        public async Task InvalidateCacheAsync(int id)
        {
            await _cache.RemoveAsync(_cachePrefix + id);
        }
        public async Task<T?> GetCacheAsync(int id)
        {
            var cached = await _cache.GetAsync(_cachePrefix + id);
            if (cached != null)
            {
                var collection = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cached));
                return collection;
            }
            return null;
        }
        public async Task<IEnumerable<T>?> GetCachedCollectionAsync()
        {
            var cached = await _cache.GetAsync(_cachePrefix + "SET");
            if (cached != null)
            {
                var deserialized = JsonConvert.DeserializeObject<IEnumerable<T>>(Encoding.UTF8.GetString(cached));
                return deserialized;
            }
            return null;
        }
        public async Task SetCacheAsync(T entity)
        {
            if (entity != null)
            {
                var serialized = JsonConvert.SerializeObject(entity);
                var encoded = Encoding.UTF8.GetBytes(serialized);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_timespan)
                };
                await _cache.SetAsync(_cachePrefix + entity.Id, encoded, options);
            }
        }
        public async Task SetCachedCollectionAsync(IEnumerable<T> entities)
        {
            if (entities != null)
            {
                var serialized = JsonConvert.SerializeObject(entities, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
                var encoded = Encoding.UTF8.GetBytes(serialized);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_timespan)
                };
                await _cache.SetAsync(_cachePrefix + "SET", encoded, options);
            }
        }
    }
}
