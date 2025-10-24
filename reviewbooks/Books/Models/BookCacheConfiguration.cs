using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReviewBooks.Books.Models
{
    public class BookCacheConfiguration : IEntityTypeConfiguration<BookCache>
    {
        public void Configure(EntityTypeBuilder<BookCache> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Id)
                .IsRequired();

            builder.Property(c => c.BookId)
                .IsRequired()
                .HasMaxLength(50); // Google volumeId

            builder.Property(c => c.JsonData)
                .IsRequired()
                .HasColumnType("nvarchar(max)"); // Store full JSON response

            builder.Property(c => c.CachedAt)
                .IsRequired();

            // Index on BookId for fast lookup
            builder.HasIndex(c => c.BookId)
                .IsUnique() // One cache per BookId
                .HasDatabaseName("IX_BookCaches_BookId");

            // Index on CachedAt for TTL queries
            builder.HasIndex(c => c.CachedAt)
                .HasDatabaseName("IX_BookCaches_CachedAt");
        }
    }
}
