using LMS_DataAccess.Data;
using LMS_DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS_DataAccess.Repositories
{
    public class EntityRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly AppDbContext _dbContext;
        public EntityRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<T> CreateAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<T> GetAsync(int id)
        {
            var entity = await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
