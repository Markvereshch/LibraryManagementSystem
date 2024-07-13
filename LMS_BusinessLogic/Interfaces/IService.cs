namespace LMS_BusinessLogic.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<T> CreateAsync(T model);
        Task DeleteAsync(T model);
        Task<T> UpdateAsync(T model, int id);
        Task<(T? OutModel, ValidationResults ValidationCode, string Message)> ValidateExistingModel(int id);
    }
}
