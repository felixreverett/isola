using FeloxGame.EntityClasses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeloxGame.UtilityClasses
{
    public class EntitySaveDataJsonConverter : JsonConverter<EntitySaveData>
    {
        public override EntitySaveData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("ItemType", out var itemTypeProperty))
                {
                    if (itemTypeProperty.GetString() == "ItemEntitySaveData")
                    {
                        return JsonSerializer.Deserialize<ItemEntitySaveData>(root.GetRawText());
                    }
                }
                return JsonSerializer.Deserialize<EntitySaveData>(root.GetRawText());
            }
        }

        public override void Write(Utf8JsonWriter writer, EntitySaveData value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
