using OpsRescue.Models;
using Microsoft.EntityFrameworkCore;

namespace OpsRescue.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<OperationsManagerProfile> OperationsManagerProfiles { get; set; }
        public DbSet<OperationsUserProfile> OperationsUserProfiles { get; set; }
        public DbSet<DeliveryPartnerProfile> DeliveryPartnerProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints on users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Operations Manager Profile (1-to-1 + Unique EmployeeId)
            modelBuilder.Entity<OperationsManagerProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<OperationsManagerProfile>()
                .HasIndex(p => p.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.OperationsManagerProfile)
                .WithOne(p => p.User)
                .HasForeignKey<OperationsManagerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Operations User Profile (1-to-1 + Unique EmployeeId)
            modelBuilder.Entity<OperationsUserProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<OperationsUserProfile>()
                .HasIndex(p => p.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.OperationsUserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<OperationsUserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Delivery Partner Profile (1-to-1 + Unique PartnerId)
            modelBuilder.Entity<DeliveryPartnerProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<DeliveryPartnerProfile>()
                .HasIndex(p => p.PartnerId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.DeliveryPartnerProfile)
                .WithOne(p => p.User)
                .HasForeignKey<DeliveryPartnerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}