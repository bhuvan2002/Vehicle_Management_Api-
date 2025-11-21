using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Vehicle>().ToTable("vehicles");

            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<Vehicle>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleId)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<Role>()
                .Property(r => r.RoleName)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.VehicleNumber)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Brand)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Model)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.AssignStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VehicleNumber)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "admin" },
                new Role { RoleId = 2, RoleName = "user" }
            );


            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.AssignedToUser)
                .WithMany(u => u.AssignedVehicles)
                .HasForeignKey(v => v.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}