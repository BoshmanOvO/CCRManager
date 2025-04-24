using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Requests
{
    public class PasswordInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("expiry")]
        public DateTime Expiry { get; set; }
    }
}
