using OpenTK.Mathematics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeloxGame.Utilities
{
    public class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string strValue = reader.GetString();
            if (strValue != null)
            {
                // Split the string into components (assuming a format like "X, Y").
                string[] parts = strValue.Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    return new Vector2(x, y);
                }
            }
            // Handle invalid or unsupported formats. You can throw an exception or return a default value.
            return Vector2.Zero; // Default value if parsing fails.
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            string strValue = $"{value.X}, {value.Y}";
            writer.WriteStringValue(strValue);
        }
    }
}
