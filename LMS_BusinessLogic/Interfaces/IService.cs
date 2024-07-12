namespace LMS_BusinessLogic.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<T> CreateAsync(T model);
        Task DeleteAsync(T model);
    }
}
