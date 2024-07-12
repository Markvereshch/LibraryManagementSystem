using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        public async Task<BookCollection> CreateAsync(BookCollection bookCollection)
        {
            await _dbContext.BookCollections.AddAsync(bookCollection);
            await _dbContext.SaveChangesAsync();
            return bookCollection;
        }
        public async Task DeleteAsync(BookCollection bookCollection)
        {
            _dbContext.BookCollections.Remove(bookCollection);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<BookCollection>> GetAllAsync()
        {
            return await _dbContext.BookCollections.Include(bc => bc.Books).ToListAsync();
        }
        public async Task<BookCollection> GetAsync(int id)
        {
            var bookCollection = await _dbContext.BookCollections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return bookCollection;
        }
    }
}
