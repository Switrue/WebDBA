using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebDBA.Converters
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (DateOnly.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }

                if (DateOnly.TryParse(value, out date))
                {
                    return date;
                }

                throw new JsonException($"Не удалось преобразовать '{value}' в DateOnly. Ожидается формат {DateFormat}");
            }

            throw new JsonException($"Неожиданный тип токена: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat));
        }
    }
}
