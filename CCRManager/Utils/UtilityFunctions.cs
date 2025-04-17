using System.Globalization;
using System.Text.Json;

namespace CommonContainerRegistry.Utils
{
    public static class UtilityFunctions
    {
        public static string ConvertToIso8601(string time)
        {
            // Define the expected input format, e.g., "MM/dd/yyyy HH:mm:ss"
            string format = "MM/dd/yyyy HH:mm:ss";
            if (DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime dateTime))
            {
                return dateTime.ToUniversalTime().ToString("o");
            }
            else
            {
                throw new ArgumentException($"Invalid date format. Please use the format: {format}");
            }
        }
        public static string PrettyPrintJson(string json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
