using AutoMapper;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Models;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;

namespace LMS_BusinessLogic.Services
{
    public class BookCollectionService : Service<BookCollectionModel, BookCollection>, IBookCollectionService
    {
        private readonly IBookCollectionRepository _collectionRepository;
        public BookCollectionService(IBookCollectionRepository collectionRepository, IMapper mapper, ICaching<BookCollection> cache)
        : base(collectionRepository, mapper, cache)
        {
            _collectionRepository = collectionRepository;
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
        public async Task<bool> HasUniqueName(string name)
        {
            var collections = await _collectionRepository.GetAllAsync();
            return !collections.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
