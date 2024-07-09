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
            modelBuilder.Entity<Book>()
            .HasOne(b => b.Collection)
            .WithMany(bc => bc.Books)
            .HasForeignKey(b => b.CollectionId)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
