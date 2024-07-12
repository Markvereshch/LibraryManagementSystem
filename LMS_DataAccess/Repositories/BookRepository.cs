using LMS_DataAccess.Data;
using LMS_DataAccess.Entities;
using LMS_DataAccess.Interfaces;
using LMS_Shared;
using Microsoft.EntityFrameworkCore;

namespace LMS_DataAccess.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;
        public BookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Book> CreateAsync(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }
        public async Task DeleteAsync(Book book)
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
        }
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
        public async Task<Book> GetAsync(int id)
        {
            var book = await _dbContext.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return book;
        }
        public async Task<Book> UpdateAsync(Book book)
        {
            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();
            return await _dbContext.Books.FirstOrDefaultAsync(u => u.Id == book.Id);
        }
    }
}
