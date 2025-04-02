using System.Globalization;
using System.Text.Json;

namespace CCRManager.Utils
{
    public static class UtilityFunctions
    {
        //public static string ConvertDateTimeToIso8601(DateTime input)
        //{
        //    // Ensure the DateTime is in UTC before formatting
        //    return input.ToUniversalTime().ToString("o");
        //}
        public static string ConvertToIso8601(string time)
        {
            // Define the expected input format, e.g., "MM/dd/yyyy HH:mm:ss"
            string format = "MM/dd/yyyy HH:mm:ss";
            DateTime dateTime;

            // Try to parse the user input according to the specified format.
            if (DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
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
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
        }
    }
}
