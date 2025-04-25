using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Requests
{
    public class TokenRequest
    {
        public required string Name { get; set; }
        public required string ScopeMapName { get; set; }
        public required string Status { get; set; }
    }
}
