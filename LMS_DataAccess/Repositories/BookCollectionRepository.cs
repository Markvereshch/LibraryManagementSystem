using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS_DataAccess.Repositories
{
    public class BookCollectionRepository : IBookCollectionRepository
    {
        private readonly AppDbContext _dbContext;
        public BookCollectionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<BookCollection>> GetAllCollectionsAsync()
        {
            return await _dbContext.BookCollections.ToListAsync();
        }
    }
}
