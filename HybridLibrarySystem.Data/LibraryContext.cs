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
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Reservation>(entity =>
            {
                entity.HasIndex(r => new { r.BookId, r.UserId }).IsUnique();
                entity.HasOne(r => r.Book)
                    .WithMany()
                    .HasForeignKey(r => r.BookId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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
