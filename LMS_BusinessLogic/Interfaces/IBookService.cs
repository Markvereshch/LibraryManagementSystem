using LMS_BusinessLogic.Models;
using LMS_Shared;

namespace LMS_BusinessLogic.Interfaces
{
    public interface IBookService : IService<BookModel>
    {
        Task<IEnumerable<BookModel>> GetAllAsync(BookFiltersModel filters);
        Task<BookModel> UpdateAsync(BookModel book, int id);
    }
}
