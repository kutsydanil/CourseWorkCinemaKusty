using Microsoft.AspNetCore.Identity;
using WebCinema.Data;
using WebCinema.Initializer;

namespace WebCinema.Middleware
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;
        public DbInitializerMiddleware(RequestDelegate next)
        {
            _next = next; 
        }
        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider, CinemaContext dbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            await DbInitializer.InitializeAsync(dbContext);

            await RoleInitializer.InitializeAsync(userManager, roleManager, applicationDbContext);

            await _next.Invoke(context);
        }
    }
}
