using System.Text.Json;
using System.Text.Json.Serialization;

namespace JobOdysseyApi.Validations;

public class NullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int intValue))
        {
            return intValue;
        }

        if (reader.TokenType == JsonTokenType.String && int.TryParse(reader.GetString(), out int parsedValue) )
        {
            return parsedValue;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value != null)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}