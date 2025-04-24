using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Payloads
{
    public class ScopeMapPropertiesPayload
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("actions")]
        public List<string>? Permissions { get; set; }
    }
}
