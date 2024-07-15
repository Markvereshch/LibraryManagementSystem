using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using LMS_Shared;
using Microsoft.EntityFrameworkCore;

namespace LMS_DataAccess.Repositories
{
    public class BookRepository : EntityRepository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext dbContext) : base(dbContext) { }
        public async Task<IEnumerable<Book>> GetAllAsync(BookFiltersModel filters)
        {
            var query = _dbContext.Books.AsQueryable();

            if (filters.Year.HasValue && filters.Year.Value >= 0)
            {
                query = query.Where(b => b.Year == filters.Year);
            }
            if (!string.IsNullOrEmpty(filters.Title))
            {
                query = query.Where(b => b.Title.ToLower().Contains(filters.Title.ToLower()));
            }
            if (!string.IsNullOrEmpty(filters.Author))
            {
                query = query.Where(b => b.Author.ToLower().Contains(filters.Author.ToLower()));
            }
            if (!string.IsNullOrEmpty(filters.Genre))
            {
                query = query.Where(b => b.Genre.ToLower().Contains(filters.Genre.ToLower()));
            }
            if (filters.Status.HasValue)
            {
                query = query.Where(b => b.Status == filters.Status);
            }
            if (filters.CollectionId.HasValue && filters.CollectionId.Value > 0)
            {
                query = query.Where(b => b.CollectionId == filters.CollectionId);
            }
            return await query.ToListAsync();
        }
    }
}
