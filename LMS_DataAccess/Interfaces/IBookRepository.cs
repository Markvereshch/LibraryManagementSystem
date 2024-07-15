using LMS_DataAccess.Entities;
using LMS_Shared;

namespace LMS_DataAccess.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetAllAsync(BookFiltersModel filters);
    }
}
