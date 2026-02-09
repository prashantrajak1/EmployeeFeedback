using FeedbackTrack.API.Data;
using FeedbackTrack.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FeedbackTrack.API.Data
{
    public static class DataSeeder
    {
        public static async Task SeedUsersAsync(AppDbContext context)
        {
            if (await context.TUsers.AnyAsync()) return;

            var adminRole = await context.TRoles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            var managerRole = await context.TRoles.FirstOrDefaultAsync(r => r.RoleName == "Manager");
            var employeeRole = await context.TRoles.FirstOrDefaultAsync(r => r.RoleName == "Employee");
            var itDept = await context.TDepartments.FirstOrDefaultAsync(d => d.DepartmentName == "IT");

            var users = new List<TUser>
            {
                new TUser { Name = "System Admin", Email = "admin@feedback.com", Password = "admin123", RoleId = adminRole.Id, DepartmentId = itDept.Id },
                new TUser { Name = "John Manager", Email = "manager@feedback.com", Password = "manager123", RoleId = managerRole.Id, DepartmentId = itDept.Id },
                new TUser { Name = "Jane Employee", Email = "employee@feedback.com", Password = "employee123", RoleId = employeeRole.Id, DepartmentId = itDept.Id }
            };

            context.TUsers.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}
