using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS_DataAccess.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;
        public BookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateBookAsync(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            IEnumerable<Book> books = await _dbContext.Books.ToListAsync();
            return books;
        }
        public async Task<Book> GetBookAsync(Expression<Func<Book, bool>> filter = null, bool isTrackable = false)
        {
            IQueryable<Book> query = _dbContext.Books;
            if(!isTrackable)
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
