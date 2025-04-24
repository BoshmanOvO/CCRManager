using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Requests
{
    public class ScopeMapRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public List<string> Permissions { get; set; } = [];
    }
}