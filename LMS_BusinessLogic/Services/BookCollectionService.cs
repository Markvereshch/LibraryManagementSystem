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
        public BookCollectionService(IBookCollectionRepository collectionRepository, IMapper mapper)
        {
            _collectionRepository = collectionRepository;
            _mapper = mapper;
        }
        public async Task<BookCollectionModel> CreateBookCollectionAsync(BookCollectionModel collectionModel)
        {
            var collection = _mapper.Map<BookCollection>(collectionModel);
            var createdCollection = await _collectionRepository.CreateBookCollectionAsync(collection);
            return _mapper.Map<BookCollectionModel>(createdCollection);
        }
        public async Task DeleteBookCollectionAsync(BookCollectionModel collectionModel)
        {
            var collection = _mapper.Map<BookCollection>(collectionModel);
            await _collectionRepository.DeleteBookCollectionAsync(collection);
        }
        public async Task<IEnumerable<BookCollectionModel>> GetAllCollectionsAsync()
        {
            var collection = await _collectionRepository.GetAllCollectionsAsync();
            return _mapper.Map<List<BookCollectionModel>>(collection);
        }
        public async Task<BookCollectionModel> GetBookCollectionAsync(int id)
        {
            var collection = await _collectionRepository.GetBookCollectionAsync(c => c.Id == id);
            return _mapper.Map<BookCollectionModel>(collection);
        }
    }
}
