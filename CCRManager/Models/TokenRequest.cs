namespace CommonContainerRegistry.Models
{
    public class TokenRequest
    {
        public required string TokenName { get; set; }
        public required string ScopeMapName { get; set; }
        public required string Status { get; set; }
    }
}
