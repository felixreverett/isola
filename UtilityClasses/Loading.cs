using FeloxGame.EntityClasses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeloxGame.UtilityClasses
{
    public static class Loading
    {
        private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions();

        static Loading()
        {
            JsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            JsonOptions.Converters.Add(new Vector2JsonConverter());
        }

        // Yes, this is from my text-based game!
        public static T LoadObject<T>(string filePath)
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath), JsonOptions)!;
        }

        public static List<T> LoadAllObjects<T>(string folderPath)
        {
            List<T> list = new List<T>();
            foreach (string s in Directory.GetFiles(folderPath))
            {
                if (s.EndsWith(".json"))
                {
                    T listItem = LoadObject<T>(s);
                    list.Add(listItem);
                }
            }
            return list;
        }

        public static void SaveObject<T>(T obj, string filePath)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(obj, JsonOptions));
        }
    }
}
