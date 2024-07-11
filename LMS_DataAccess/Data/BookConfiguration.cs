using LMS_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS_DataAccess.Data
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Title")
                .HasComment("Title of the book");
            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Author")
                .HasComment("Author of the book");
            builder.Property(b => b.Genre)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Genre")
                .HasComment("Genre of the book");
            builder.Property(b => b.Year)
                .IsRequired()
                .HasColumnName("Year")
                .HasComment("Year of publication");
            builder.Property(b => b.Status)
                .IsRequired()
                .HasColumnName("Status")
                .HasComment("Status of the book");
            builder.HasOne(b => b.Collection)
               .WithMany(bc => bc.Books)
               .HasForeignKey(b => b.CollectionId)
               .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
