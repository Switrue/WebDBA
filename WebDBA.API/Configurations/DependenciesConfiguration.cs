using WebDBA.API.Interfaces;
using WebDBA.API.Services;

namespace WebDBA.API.Configurations
{
    public static class DependenciesConfiguration
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IValidationService, ValidationService>();
        }
    }
}
