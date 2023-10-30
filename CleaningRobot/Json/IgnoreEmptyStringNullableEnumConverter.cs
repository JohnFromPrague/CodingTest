using System.Text.Json;
using System.Text.Json.Serialization;

namespace CleaningRobot.Json
{

    public class IgnoreEmptyStringNullableEnumConverter<T> : JsonConverter<T?>
        where T : struct
    {
        private readonly JsonConverter<T> underlyingConverter;

        public IgnoreEmptyStringNullableEnumConverter(JsonConverter<T> underlyingConverter)
        {
            this.underlyingConverter = underlyingConverter ?? throw new ArgumentNullException(nameof(underlyingConverter));
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader is { TokenType: JsonTokenType.Null }
                       or { TokenType: JsonTokenType.String, ValueSpan.Length: 0 })
            {
                return null;
            }

            return this.underlyingConverter.Read(ref reader, typeof(T), options);
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            this.underlyingConverter.Write(writer, value.Value, options);
        }
    }
}
