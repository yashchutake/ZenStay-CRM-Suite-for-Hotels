using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ZenHotelManagement.WebApp.IdentityDbContex
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret@123";
        private const string receptionistUser = "Receptionist";
        private const string receptionistPassword = "Secret@123";
        private const string adminRole = "Administrator";
        private const string receptionistRole = "Receptionist";

        public static async Task SeedData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                }

                // Seed Roles
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                }

                if (!await roleManager.RoleExistsAsync(receptionistRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(receptionistRole));
                }

                // Seed Admin User
                IdentityUser? admin = await userManager.FindByNameAsync(adminUser);
                if (admin == null)
                {
                    admin = new IdentityUser(adminUser)
                    {
                        Email = "admin@example.com",
                        PhoneNumber = "1234567890"
                    };
                    await userManager.CreateAsync(admin, adminPassword);
                    await userManager.AddToRoleAsync(admin, adminRole);
                }

                // Seed Receptionist User
                IdentityUser? receptionist = await userManager.FindByNameAsync(receptionistUser);
                if (receptionist == null)
                {
                    receptionist = new IdentityUser(receptionistUser)
                    {
                        Email = "receptionist@example.com",
                        PhoneNumber = "0987654321"
                    };
                    await userManager.CreateAsync(receptionist, receptionistPassword);
                    await userManager.AddToRoleAsync(receptionist, receptionistRole);
                }
            }
        }
    }

}
