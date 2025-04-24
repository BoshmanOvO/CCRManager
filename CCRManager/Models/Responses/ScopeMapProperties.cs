using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Responses
{
    public class ScopeMapProperties
    {
        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("actions")]
        public List<string> Permissions { get; set; } = [];
    }
}
