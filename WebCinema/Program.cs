using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebCinema.Data;
using WebCinema.Middleware;
using WebCinema.Validators;

namespace WebCinema
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<CinemaContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddTransient<IUserValidator<IdentityUser>, CustomUserValidator>();

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
                     options.SignIn.RequireConfirmedAccount = false;
                     options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders().AddErrorDescriber<CustomIdentityErrorDescriber>();

            builder.Services.AddTransient<CacheService.CacheProvider>();
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseMiddleware<DbInitializerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}