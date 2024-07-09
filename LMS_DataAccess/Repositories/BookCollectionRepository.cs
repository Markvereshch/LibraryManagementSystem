using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace LMS_DataAccess.Repositories
{
    public class BookCollectionRepository : IBookCollectionRepository
    {
        private readonly AppDbContext _dbContext;
        public BookCollectionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<BookCollection> CreateBookCollectionAsync(BookCollection bookCollection)
        {
            await _dbContext.BookCollections.AddAsync(bookCollection);
            await _dbContext.SaveChangesAsync();
            return bookCollection;
        }
        public async Task DeleteBookCollectionAsync(BookCollection bookCollection)
        {
            _dbContext.BookCollections.Remove(bookCollection);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<BookCollection>> GetAllCollectionsAsync()
        {
            return await _dbContext.BookCollections.Include(bc => bc.Books).ToListAsync();
        }
        public async Task<BookCollection> GetBookCollectionAsync(Expression<Func<BookCollection, bool>> filter, bool isTrackable = false)
        {
            IQueryable<BookCollection> query = _dbContext.BookCollections;
            if (!isTrackable)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }
    }
}
