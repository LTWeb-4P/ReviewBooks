using ReviewBooks.Books.Models;
using Microsoft.EntityFrameworkCore;
using ReviewBooks.Reviews.Models;
using ReviewBooks.Users.Models;
using ReviewBooks.Forum.Model;

namespace ReviewBooks.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Các DbSet tương ứng với các bảng
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCache> BookCaches { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<ForumComment> ForumComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all IEntityTypeConfiguration from assembly (BookConfiguration, BookCacheConfiguration, etc.)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Relationship: Review.BookId (string) -> Book.Id (string)
            // Quan hệ 1-nhiều: Book - Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-nhiều: User - Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ nhiều-nhiều: User - FavoriteBooks
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteBooks)
                .WithMany(b => b.FavoritedByUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserFavoriteBooks",
                    j => j.HasOne<Book>().WithMany().HasForeignKey("BookId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "BookId");
                        j.ToTable("UserFavoriteBooks");
                        j.Property<DateTime>("CreatedAt").HasDefaultValueSql("GETUTCDATE()");
                    });

            // Quan hệ 1-nhiều: User - ForumPost
            modelBuilder.Entity<ForumPost>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-nhiều: ForumPost - ForumComment
            modelBuilder.Entity<ForumComment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-nhiều: User - ForumComment
            modelBuilder.Entity<ForumComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete cycles
        }
    }
}
