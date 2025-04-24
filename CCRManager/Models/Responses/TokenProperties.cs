using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Responses
{
    public class TokenProperties
    {
        [JsonPropertyName("creationDate")]
        public DateTime? CreationDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
