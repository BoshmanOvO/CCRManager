using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Payloads
{
    public class TokenRequestPayload
    {
        [JsonPropertyName("properties")]
        public TokenPropertiesPayload? TokenProperties { get; set; }
    }
}
