using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS_DataAccess.Repositories
{
    public class BookCollectionRepository : EntityRepository<BookCollection>, IBookCollectionRepository
    {
        public BookCollectionRepository(AppDbContext dbContext) : base(dbContext) { }
        public async Task<IEnumerable<BookCollection>> GetAllAsync()
        {
            return await _dbContext.BookCollections.AsNoTracking().Include(bc => bc.Books).ToListAsync();
        }
    }
}
