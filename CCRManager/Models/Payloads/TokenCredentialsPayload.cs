namespace CommonContainerRegistry.Models.Payloads
{
    public class TokenCredentialsPayload
    {
        public Array Passwords { get; set; } = Array.Empty<string>();
    }
}