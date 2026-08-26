using WebDBA.Configuration;
using WebDBA.Interfaces;
using WebDBA.Services;

namespace WebDBA.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            var apiSettings = new ApiSettings();
            configuration.GetSection("ApiSettings").Bind(apiSettings);
            services.AddSingleton(apiSettings);

            services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri(apiSettings.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", apiSettings.AcceptHeader);
                client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
            });

            return services;
        }
    }
}
