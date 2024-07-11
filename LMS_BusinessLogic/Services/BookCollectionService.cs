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
        private readonly IBookCollectionCaching _cache;
        public BookCollectionService(IBookCollectionRepository collectionRepository, IMapper mapper, IBookCollectionCaching cache)
        {
            _collectionRepository = collectionRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<BookCollectionModel> CreateBookCollectionAsync(BookCollectionModel collectionModel)
        {
            var collection = _mapper.Map<BookCollection>(collectionModel);
            var createdCollection = await _collectionRepository.CreateBookCollectionAsync(collection);
            await _cache.DeleteCollectionAsync(createdCollection.Id);
            return _mapper.Map<BookCollectionModel>(createdCollection);
        }
        public async Task DeleteBookCollectionAsync(BookCollectionModel collectionModel)
        {
            var collection = _mapper.Map<BookCollection>(collectionModel);
            await _collectionRepository.DeleteBookCollectionAsync(collection);
            await _cache.DeleteCollectionAsync(collectionModel.Id);
        }
        public async Task<IEnumerable<BookCollectionModel>> GetAllCollectionsAsync()
        {
            var collection = await _cache.GetBookCollectionsAsync();
            if (collection == null)
            {
                collection = await _collectionRepository.GetAllCollectionsAsync();
                await _cache.SetBookCollectionsAsync(collection);
            }
            return _mapper.Map<List<BookCollectionModel>>(collection);
        }
        public async Task<BookCollectionModel> GetBookCollectionAsync(int id)
        {
            var collection = await _cache.GetBookCollectionAsync(id);
            if (collection == null)
            {
                collection = await _collectionRepository.GetBookCollectionAsync(c => c.Id == id);
                await _cache.SetBookCollectionAsync(collection);
            }
            return _mapper.Map<BookCollectionModel>(collection);
        }
    }
}
