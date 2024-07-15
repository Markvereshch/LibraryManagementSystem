using LMS_DataAccess.Entities;

namespace LMS_DataAccess.Interfaces
{
    public interface IBookCollectionRepository : IRepository<BookCollection>
    {
        Task<IEnumerable<BookCollection>> GetAllAsync();
    }
}
