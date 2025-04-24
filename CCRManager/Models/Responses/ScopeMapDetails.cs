using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Responses
{
    public class ScopeMapDetails
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("properties")]
        public ScopeMapProperties? Properties { get; set; }
    }
}
