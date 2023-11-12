using Newtonsoft.Json;
using System.IO;

namespace MyBeatSaberScore.Utility
{
    internal static class Json
    {
        public static T? DeserializeFromLocalFile<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            using var sr = File.OpenText(path);
            using var reader = new JsonTextReader(sr);
            var serializer = new JsonSerializer();
            var deserialized = serializer.Deserialize<T>(reader);
            return deserialized;
        }

        public static void SerializeToLocalFile<T>(T data, string path, Formatting format = Formatting.Indented)
        {
            var dir = System.IO.Path.GetDirectoryName(path);
            if (dir != null) Directory.CreateDirectory(dir);
            var setting = new JsonSerializerSettings()
            {
                Formatting = format,
                DateFormatString = "yyyy-MM-ddTHH:mm:sszzz"
            };
            var jsonString = JsonConvert.SerializeObject(data, setting);
            File.WriteAllText(path, jsonString, System.Text.Encoding.UTF8);
        }
    }
}
