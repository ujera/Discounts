using Discounts.Domain.Entities;
using Discounts.Persistance.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Discounts.Persistance.Seed
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<DiscountsManagementContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                Migrate(context);

         
                SeedEverything(context, userManager, roleManager).Wait();
            }
            catch (Exception ex)
            {
                // Log errors here if you have a logger
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
        }

        private static void Migrate(DiscountsManagementContext context)
        {
            context.Database.Migrate();
        }

        private static async Task SeedEverything(DiscountsManagementContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAsync(roleManager).ConfigureAwait(false);
            await SeedUsersAsync(userManager).ConfigureAwait(false);
            SeedCategories(context);
            SeedSettings(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Merchant", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                {
                    await roleManager.CreateAsync(new IdentityRole(role)).ConfigureAwait(false);
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {

            var adminEmail = "admin@discounts.ge";
            var adminUser = await userManager.FindByEmailAsync(adminEmail).ConfigureAwait(false);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    IsActive = true,
                    EmailConfirmed = true
                };

 
                var result = await userManager.CreateAsync(newAdmin, "Admin123!").ConfigureAwait(false);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin").ConfigureAwait(false);
                }
            }
        }

        private static void SeedCategories(DiscountsManagementContext context)
        {
            if (context.Categories.Any()) return;

            var categories = new List<Category>
            {
                new Category { Name = "Food & Dining" },
                new Category { Name = "Entertainment" },
                new Category { Name = "Travel" },
                new Category { Name = "Electronics" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        private static void SeedSettings(DiscountsManagementContext context)
        {
            if (context.SystemSettings.Any()) return;

            context.SystemSettings.Add(new SystemSetting
            {
                ReservationDurationInMinutes = 30,
                MerchantEditWindowInHours = 24
            });
            context.SaveChanges();
        }
    }
}

