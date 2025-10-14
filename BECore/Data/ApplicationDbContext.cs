using BECore.Books.Models;
using Microsoft.EntityFrameworkCore;


namespace BECore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Các DbSet tương ứng với các bảng
        public DbSet<BECore.Users.Models.User> Users { get; set; }
        public DbSet<BECore.Books.Models.Book> Books { get; set; }
        public DbSet<BECore.Books.Models.BookCaches> BookCaches { get; set; }
        public DbSet<BECore.Reviews.Models.Review> Reviews { get; set; }
        public DbSet<BECore.Favorite.Models.Favorite> Favorites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Áp dụng cấu hình riêng từng entity
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<BookCaches>().HasNoKey();

        }
    }
}
