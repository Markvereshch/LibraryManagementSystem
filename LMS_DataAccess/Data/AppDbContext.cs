using LMS_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS_DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCollection> BookCollections { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasData(
                new Book()
                {
                  Id = 1, Author = "Good Boi", Genre = "Some genre", Status = BookStatus.Available, Title = "Good book", Year = 1941
                }
            );
        }
    }
}
