using LMS_BusinessLogic.Models;

namespace LMS_BusinessLogic.Interfaces
{
    public interface IBookCollectionService : IService<BookCollectionModel>
    {
        Task<IEnumerable<BookCollectionModel>> GetAllAsync();
        Task<bool> HasUniqueName(string name);
    }
}
