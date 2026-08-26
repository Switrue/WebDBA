using Microsoft.EntityFrameworkCore;
using WebDBA.Migrator.Migration;

namespace WebDBA.API.Configurations
{
    public static class DbContextConfiguration
    {
        public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options
                    .UseNpgsql(configuration.GetConnectionString(nameof(AppDbContext)))
                    .EnableSensitiveDataLogging(false)
                    .LogTo(Console.WriteLine, LogLevel.Information);
            });
        }
    }
}
