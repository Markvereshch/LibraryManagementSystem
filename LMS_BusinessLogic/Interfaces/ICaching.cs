namespace LMS_BusinessLogic.Interfaces
{
    public interface ICaching<T> where T : class
    {
        Task<T?> GetCacheAsync(int id);
        Task SetCacheAsync(T entity);
        Task<IEnumerable<T>?> GetCachedCollectionAsync();
        Task SetCachedCollectionAsync(IEnumerable<T> entities);
        Task InvalidateCacheAsync(int id);
        Task InvalidateCachedCollectionAsync();
    }
}
