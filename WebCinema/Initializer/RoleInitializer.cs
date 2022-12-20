using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebCinema.Data;

namespace WebCinema.Initializer
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            await applicationDbContext.Database.MigrateAsync();

            string adminEmail = "admin@gmail.com";
            string password = "dANIK_2003";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("employee") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("employee"));
            }

            IdentityUser admin = userManager.Users.ToList().
                                 FirstOrDefault(u => userManager.GetRolesAsync(u).Result.Contains("admin"));

            if (admin == null)
            {
                admin = new IdentityUser { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(admin);
                    await userManager.ConfirmEmailAsync(admin, code);
                }
            }
        }
    }
}
