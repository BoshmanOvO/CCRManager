using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Payloads
{
    public class TokenPasswordRequestPayload
    {
        [JsonPropertyName("tokenId")]
        public required string TokenId { get; set; }

        [JsonPropertyName("expiry")]
        public required string ExpiryDate { get; set; }
    }
}
