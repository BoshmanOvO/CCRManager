using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Responses
{
    public class ScopeMapResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("properties")]
        public ScopeMapProperties Properties { get; set; } = new ScopeMapProperties();
    }
}
