using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Payloads
{
    public class ScopeMapRequestPayload
    {
        [JsonPropertyName("properties")]
        public ScopeMapPropertiesPayload? Properties { get; set; }
    }
}
