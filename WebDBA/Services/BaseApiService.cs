using System.Text;
using System.Text.Json;
using WebDBA.Helpers;

namespace WebDBA.Services
{
    public class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected BaseApiService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected async Task<(bool Success, T? Data, string? ErrorMessage)> SendAsync<T>(
            HttpMethod method,
            string url,
            object? content = null,
            bool returnData = false)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);

                if (content != null)
                {
                    request.Content = new StringContent(
                        JsonSerializer.Serialize(content, _jsonOptions),
                        Encoding.UTF8,
                        "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (!returnData || typeof(T) == typeof(object))
                        return (true, default, null);

                    var data = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                    return (true, data, null);
                }

                var error = ApiErrorHelper.ExtractErrorMessage(responseContent);
                _logger.LogWarning("Ошибка {Method} {Url}: {StatusCode} - {Error}", method, url, response.StatusCode, error);
                return (false, default, error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка {Method} {Url}", method, url);
                return (false, default, $"Ошибка: {ex.Message}");
            }
        }

        protected async Task<(bool Success, string? ErrorMessage)> SendAsync(
            HttpMethod method,
            string url,
            object? content = null) =>
            (await SendAsync<object>(method, url, content, false)).ToTuple();

        protected async Task<(bool Success, T? Data, string? ErrorMessage)> GetAsync<T>(string url) =>
            await SendAsync<T>(HttpMethod.Get, url, null, true);

        protected async Task<(bool Success, T? Data, string? ErrorMessage)> PostAsync<T>(
            string url,
            object? content = null) =>
            await SendAsync<T>(HttpMethod.Post, url, content, true);

        protected async Task<(bool Success, T? Data, string? ErrorMessage)> PutAsync<T>(
            string url,
            object? content = null) =>
            await SendAsync<T>(HttpMethod.Put, url, content, true);
    }
}