namespace CommonContainerRegistry.Models.Payloads
{
    public class TokenPropertiesPayload
    {
        public string ScopeMapId { get; set; } = string.Empty;
        public string? Status { get; set; }
        public TokenCredentialsPayload TokenCredentials { get; set; } = new TokenCredentialsPayload();
    }
}