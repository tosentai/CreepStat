using CreepStat.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CreepStat.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.SteamID); 

            modelBuilder.Entity<LoginHistory>()
                .HasOne(lh => lh.User) 
                .WithMany(u => u.LoginHistories)
                .HasForeignKey(lh => lh.SteamID)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
