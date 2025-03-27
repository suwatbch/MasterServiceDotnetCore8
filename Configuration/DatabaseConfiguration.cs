using EtaxService.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EtaxService.Configuration
{
    public static class DatabaseConfiguration
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // ตั้งค่า Default Database
            var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(defaultConnectionString))
            {
                throw new InvalidOperationException("Default connection string is not configured.");
            }
            services.AddDbContext<DefaultDatabaseContext>(options =>
                options.UseSqlServer(defaultConnectionString));

            // ตั้งค่า Etax Database
            var etaxConnectionString = configuration.GetConnectionString("EtaxConnection");
            if (string.IsNullOrEmpty(etaxConnectionString))
            {
                throw new InvalidOperationException("Etax connection string is not configured.");
            }
            services.AddDbContext<EtaxDatabaseContext>(options =>
                options.UseSqlServer(etaxConnectionString));
        }
    }
}
