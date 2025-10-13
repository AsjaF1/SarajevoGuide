using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SarajevoGuide.Models;

namespace SarajevoGuide.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recenzija> Recenzija { get; set; } = null!;
        public DbSet<RegistrovaniKorisnik> RegistrovaniKorisnik { get; set; } = null!;
        public DbSet<Kupovina> Kupovina { get; set; } = null!;
        public DbSet<Event> Event { get; set; } = null!;
        public DbSet<Bookmark> Bookmark { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Recenzija>().ToTable("Recenzija");
            builder.Entity<RegistrovaniKorisnik>().ToTable("RegistrovaniKorisnik");
            builder.Entity<Kupovina>().ToTable("Kupovina");
            builder.Entity<Event>().ToTable("Event");
            builder.Entity<Bookmark>().ToTable("Bookmark");

            // Fix Identity column types for MySQL
            builder.Entity<IdentityUser>(entity =>
            {
                entity.Property(u => u.Id).HasMaxLength(191);
                entity.Property(u => u.UserName).HasMaxLength(191);
                entity.Property(u => u.NormalizedUserName).HasMaxLength(191);
                entity.Property(u => u.Email).HasMaxLength(191);
                entity.Property(u => u.NormalizedEmail).HasMaxLength(191);
                entity.Property(u => u.ConcurrencyStamp).HasColumnType("longtext");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(r => r.Id).HasMaxLength(191);
                entity.Property(r => r.Name).HasMaxLength(191);
                entity.Property(r => r.NormalizedName).HasMaxLength(191);
                entity.Property(r => r.ConcurrencyStamp).HasColumnType("longtext");
            });
        }
    }
}
