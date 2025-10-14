using Microsoft.EntityFrameworkCore;
using BECore.Books.Models;
using BECore.Users.Models;
using BECore.Reivews.Models;


namespace BECore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Các DbSet tương ứng với các bảng
        public DbSet<BECore.Users.Models.User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCaches> BookCaches { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BECore.Favorite.Models.Favorite> Favorites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Áp dụng cấu hình riêng từng entity
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<BookCache>().HasNoKey();

        }
    }
}
