using System.Text.Json;
using WebDBA.Converters;

namespace WebDBA.Configuration
{
    public class JsonSettings
    {
        public static void ConfigureJsonOptions(Microsoft.AspNetCore.Http.Json.JsonOptions options)
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        }
    }
}
