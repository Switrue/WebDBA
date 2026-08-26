using System.Text.Json;

namespace WebDBA.Helpers
{
    public static class ApiErrorHelper
    {
        public static string ExtractErrorMessage(string errorContent)
        {
            try
            {
                using var document = JsonDocument.Parse(errorContent);
                var root = document.RootElement;

                var messages = new List<string>();

                if (root.TryGetProperty("errors", out var errorsElementLower) && errorsElementLower.ValueKind == JsonValueKind.Object)
                {
                    messages.AddRange(ExtractMessagesFromErrors(errorsElementLower));
                }

                if (messages.Any())
                {
                    return string.Join("; ", messages);
                }

                if (root.TryGetProperty("error", out var errorElementLower))
                {
                    var msg = errorElementLower.GetString();
                    if (!string.IsNullOrEmpty(msg))
                        return msg;
                }

                if (root.TryGetProperty("success", out var successElement))
                {
                    var msg = successElement.GetString();
                    if (!string.IsNullOrEmpty(msg))
                        return msg;
                }

                return errorContent;
            }
            catch (JsonException)
            {
                return errorContent;
            }
        }

        private static List<string> ExtractMessagesFromErrors(JsonElement errorsElement)
        {
            var messages = new List<string>();

            foreach (var property in errorsElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        var msg = item.GetString();
                        if (!string.IsNullOrEmpty(msg))
                            messages.Add(msg);
                    }
                }
                else if (property.Value.ValueKind == JsonValueKind.String)
                {
                    var msg = property.Value.GetString();
                    if (!string.IsNullOrEmpty(msg))
                        messages.Add(msg);
                }
                else if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    messages.AddRange(ExtractMessagesFromErrors(property.Value));
                }
            }

            return messages;
        }

        public static (bool Success, string? ErrorMessage) ToTuple<T>(
            this (bool Success, T? Data, string? ErrorMessage) result) =>
            (result.Success, result.ErrorMessage);
    }
}