using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReviewBooks.Books.Models
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Primary Key
            builder.HasKey(b => b.Id);

            // Properties
            builder.Property(b => b.Id)
                .IsRequired()
                .HasMaxLength(50); // Google volumeId typical length

            builder.Property(b => b.Title)
                .HasMaxLength(500);

            builder.Property(b => b.Authors)
                .HasMaxLength(500);

            builder.Property(b => b.Publisher)
                .HasMaxLength(300);

            builder.Property(b => b.Description)
                .HasMaxLength(4000);

            builder.Property(b => b.Thumbnail)
                .HasMaxLength(500);

            builder.Property(b => b.AverageRating)
                .HasPrecision(3, 2); // e.g. 4.50

            builder.Property(b => b.CachedAt)
                .IsRequired();

            builder.Property(b => b.PublishedDate)
                .IsRequired(false);

            builder.Property(b => b.ISBN)
            .HasMaxLength(20);

            builder.Property(b => b.Categories)
            .HasMaxLength(200);

            builder.Property(b => b.Price)
                .HasPrecision(10, 2); // e.g. 19.99
                
            builder.Property(b => b.Url_Buy)
                .HasMaxLength(500);

            // Index for performance
            builder.HasIndex(b => b.CachedAt)
                .HasDatabaseName("IX_Books_CachedAt");

            builder.HasIndex(b => b.Title)
                .HasDatabaseName("IX_Books_Title");

            // Relationships configured in ApplicationDbContext or here
            // Already configured: Review.BookId -> Book.Id
            // Already configured: User.FavoriteBooks <-> Book.FavoritedByUsers
        }
    }
}
