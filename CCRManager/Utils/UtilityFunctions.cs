using System.Text.Json;

namespace CCRManager.Utils
{
    public class UtilityFunctions
    {
        public static string ConvertDateTimeStringToIso8601(long epochMilliseconds)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(epochMilliseconds);
            string formattedDate = dateTimeOffset.UtcDateTime.ToString("o");
            return formattedDate;
        }
        public static string PrettyPrintJson(string json)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
        }
        public async Task<string> FormatResponseAsync(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            string prettyJson = UtilityFunctions.PrettyPrintJson(responseContent);
            return prettyJson;
        }
    }
}
