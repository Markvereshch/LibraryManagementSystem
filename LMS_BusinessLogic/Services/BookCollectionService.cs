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
            await _cache.InvalidateCachedCollectionAsync();
            return _mapper.Map<BookCollectionModel>(createdCollection);
        }
        public async Task DeleteAsync(BookCollectionModel collectionModel)
        {
            var collection = _mapper.Map<BookCollection>(collectionModel);
            await _collectionRepository.DeleteAsync(collection);
            await _cache.InvalidateCacheAsync(collectionModel.Id);
            await _cache.InvalidateCachedCollectionAsync();
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
        public async Task<BookCollectionModel> UpdateAsync(BookCollectionModel model, int id)
        {
            var collectionToUpdate = await _cache.GetCacheAsync(id);
            if (collectionToUpdate == null)
            {
                collectionToUpdate = await _collectionRepository.GetAsync(id);
            }
            _mapper.Map(model, collectionToUpdate);
            collectionToUpdate.Id = id;
            var updatedCollection = await _collectionRepository.UpdateAsync(collectionToUpdate);

            await _cache.InvalidateCacheAsync(id);
            await _cache.InvalidateCachedCollectionAsync();
            return _mapper.Map<BookCollectionModel>(updatedCollection);
        }
        public async Task<(BookCollectionModel?, ValidationResults, string)> ValidateExistingModel(int id)
        {
            if (id <= 0)
            {
                return (null, ValidationResults.BadRequest, $"Invalid book collection ID ({id} is less than 1)");
            }
            var collection = await GetAsync(id);
            if (collection == null)
            {
                return (null, ValidationResults.NotFound, $"Book collection with ID={id} cannot be found");
            }
            var collectionModel = _mapper.Map<BookCollectionModel>(collection);
            return (collectionModel, ValidationResults.OK, "OK");
        }
        public async Task<bool> HasUniqueName(string name)
        {
            var collections = await _collectionRepository.GetAllAsync();
            return !collections.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
