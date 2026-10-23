using HybridLibrarySystem.Core.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HybridLibrarySystem.Data
{
    public class LibraryContext(DbContextOptions<LibraryContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
            if (!options.IsConfigured)
            {
                //options.UseSqlite($"Data Source={DbPath}");
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LibraryContext;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }
    }
}
