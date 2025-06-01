using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using WareHouseManager.Repositories;
namespace WareHouseManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<UserRepository>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                return new UserRepository(connectionString);
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<WareHouseManager.Repositories.ProductRepository>();
            builder.Services.AddScoped<WareHouseManager.Repositories.SupplierRepository>();
            builder.Services.AddScoped<CustomerRepository>();
            builder.Services.AddScoped<WarehouseManagerRepository>();
            builder.Services.AddScoped<SalesManagerRepository>();
            builder.Services.AddScoped<WareHouseManager.Repositories.TransactionInRepository>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
