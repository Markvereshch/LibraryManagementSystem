using LMS_BusinessLogic.Interfaces;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;

namespace LMS_BusinessLogic.Services
{
    public class BookCollectionService : IBookCollectionService
    {
        private readonly IBookCollectionRepository _collectionRepository;
        public BookCollectionService(IBookCollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }
        public async Task<IEnumerable<BookCollection>> GetAllCollectionsAsync()
        {
            return await _collectionRepository.GetAllCollectionsAsync();
        }
    }
}
