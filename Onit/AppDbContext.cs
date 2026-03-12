using Microsoft.EntityFrameworkCore;
using Onit.models;

namespace Onit
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Server> Servers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserServer> UserServers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = Environment.GetEnvironmentVariable("DB_CONNECTION");

            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserServer>()
                .HasKey(us => new { us.UserId, us.ServerId });

            modelBuilder.Entity<UserServer>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserServers)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserServer>()
                .HasOne(us => us.Server)
                .WithMany(s => s.UserServers)
                .HasForeignKey(us => us.ServerId);
        }
    }
}