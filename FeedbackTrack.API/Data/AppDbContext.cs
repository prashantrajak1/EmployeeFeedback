using FeedbackTrack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FeedbackTrack.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TRole> TRoles { get; set; }
        public DbSet<TDepartment> TDepartments { get; set; }
        public DbSet<TUser> TUsers { get; set; }
        public DbSet<TFeedback> TFeedbacks { get; set; }
        public DbSet<TRecognition> TRecognitions { get; set; }
        public DbSet<TReview> TReviews { get; set; }
        public DbSet<TNotification> TNotifications { get; set; }

        // Views and SP results
        public DbSet<vw_UserProfileView> UserProfiles { get; set; }
        public DbSet<sp_UserFeedbackStats> UserFeedbackStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<TFeedback>()
                .HasOne(f => f.FromUser)
                .WithMany()
                .HasForeignKey(f => f.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TFeedback>()
                .HasOne(f => f.ToUser)
                .WithMany()
                .HasForeignKey(f => f.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<TRecognition>()
                .HasOne(r => r.FromUser)
                .WithMany()
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TRecognition>()
                .HasOne(r => r.ToUser)
                .WithMany()
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Seed defaults for Roles and Departments
            modelBuilder.Entity<TRole>().HasData(
                new TRole { Id = 1, RoleName = "Admin" },
                new TRole { Id = 2, RoleName = "Manager" },
                new TRole { Id = 3, RoleName = "Employee" }
            );

            modelBuilder.Entity<TDepartment>().HasData(
                new TDepartment { Id = 1, DepartmentName = "IT" },
                new TDepartment { Id = 2, DepartmentName = "HR" },
                new TDepartment { Id = 3, DepartmentName = "Sales" }
            );

            // Configure Views and Keyless Entities
            modelBuilder.Entity<vw_UserProfileView>(eb =>
            {
                eb.HasNoKey();
                eb.ToView("vw_UserProfileView");
            });

            modelBuilder.Entity<sp_UserFeedbackStats>(eb =>
            {
                eb.HasNoKey();
            });
        }
    }
}
