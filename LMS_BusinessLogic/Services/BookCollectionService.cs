using AutoMapper;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Models;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;

namespace LMS_BusinessLogic.Services
{
    public class BookCollectionService : IBookCollectionService
    {
        private readonly IBookCollectionRepository _collectionRepository;
        private readonly IMapper _mapper;
        private readonly ICaching<BookCollection> _cache;
        public BookCollectionService(IBookCollectionRepository collectionRepository, IMapper mapper, ICaching<BookCollection> cache)
        {
            _collectionRepository = collectionRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<BookCollectionModel> CreateAsync(BookCollectionModel collectionModel)
        {
            var collection = _mapper.Map<BookCollection>(collectionModel);
            var createdCollection = await _collectionRepository.CreateAsync(collection);
            await _cache.InvalidateCacheAsync(createdCollection.Id);
            return _mapper.Map<BookCollectionModel>(createdCollection);
        }
        public async Task DeleteAsync(BookCollectionModel collectionModel)
        {
            var collection = _mapper.Map<BookCollection>(collectionModel);
            await _collectionRepository.DeleteAsync(collection);
            await _cache.InvalidateCacheAsync(collectionModel.Id);
        }
        public async Task<IEnumerable<BookCollectionModel>> GetAllAsync()
        {
            var collection = await _cache.GetCachedCollectionAsync();
            if (collection == null)
            {
                collection = await _collectionRepository.GetAllAsync();
                await _cache.SetCachedCollectionAsync(collection);
            }
            return _mapper.Map<List<BookCollectionModel>>(collection);
        }
        public async Task<BookCollectionModel> GetAsync(int id)
        {
            var collection = await _cache.GetCacheAsync(id);
            if (collection == null)
            {
                collection = await _collectionRepository.GetAsync(id);
                await _cache.SetCacheAsync(collection);
            }
            return _mapper.Map<BookCollectionModel>(collection);
        }
        public async Task UpdateAsync(int id)
        {
            await _cache.InvalidateCacheAsync(id);
            await _cache.InvalidateCachedCollectionAsync();
        }
    }
}
