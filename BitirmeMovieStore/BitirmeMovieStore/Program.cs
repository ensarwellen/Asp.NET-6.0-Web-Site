using BitirmeMovieStore.Entities;
using BitirmeMovieStore.Repositories.Abstract;
using BitirmeMovieStore.Repositories.Implementation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BitirmeMovieStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddScoped<IGenreService, GenreService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IMovieService, MovieService>();

            builder.Services.AddDbContext<DatabaseContext>(opts =>
            {
                opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                //opts.UseLazyLoadingProxies();
            });

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opts =>
                {
                    opts.Cookie.Name = ".BitirmeMovieStore.auth";
                    opts.ExpireTimeSpan = TimeSpan.FromDays(7);
                    opts.SlidingExpiration = false;
                    opts.LoginPath = "/Account/Login";
                    opts.LogoutPath = "/Account/Logout";
                    opts.AccessDeniedPath = "/Home/AccessDenied";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}