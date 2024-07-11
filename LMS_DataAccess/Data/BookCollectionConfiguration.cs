using LMS_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS_DataAccess.Data
{
    internal class BookCollectionConfiguration : IEntityTypeConfiguration<BookCollection>
    {
        public void Configure(EntityTypeBuilder<BookCollection> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Name")
                .HasComment("Name of the book collection");
            builder.HasMany(c => c.Books)
                .WithOne(b => b.Collection)
                .HasForeignKey(b => b.CollectionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
