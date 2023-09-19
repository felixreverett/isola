using System.Text.Json;
using System.Xml;

namespace FeloxGame.Core
{
    public static class Loading
    {
        // Yes, this is from my text-based game!
        public static T LoadObject<T>(string filePath)
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath))!;
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
            File.WriteAllText(filePath, JsonSerializer.Serialize(obj));
        }
    }
}
