using System.Text.Json;

namespace CoreLibrary.Shared.Helpers
{
    public static class JsonHelper
    {
        private static JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(this object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T? Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
