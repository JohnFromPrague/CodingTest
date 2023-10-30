using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CleaningRobot.Json
{
    public class IgnoreEmptyStringNullableEnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsValueType ||
                !typeToConvert.IsGenericType ||
                typeToConvert.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return false;
            }

            Type underlyingType = typeToConvert.GetGenericArguments()[0];
            return underlyingType.IsEnum; // Replace with whatever check is appropriate
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(this.CanConvert(typeToConvert));
            Type underlyingType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(IgnoreEmptyStringNullableEnumConverter<>).MakeGenericType(underlyingType);
            JsonConverter underlyingConverter = options.GetConverter(underlyingType);
            return (JsonConverter)Activator.CreateInstance(converterType, underlyingConverter)!;
        }
    }
}
