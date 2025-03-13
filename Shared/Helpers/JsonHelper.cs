using System.Text.Json;

namespace CoreLibrary.Shared.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(this object obj)
        {

            return JsonSerializer.Serialize(obj);
        }

        public static string Serialize(this object obj, JsonSerializerOptions? options = null)
        {

            return JsonSerializer.Serialize(obj, options);
        }

        public static T? Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public static T? Deserialize<T>(this string json, JsonSerializerOptions? options = null)
        {
            options ??= _options;

            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
