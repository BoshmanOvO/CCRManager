namespace CommonContainerRegistry.Models.Requests
{
    public class PasswordRequest
    {
        public required string TokenName {  get; set; }
        public required string PasswordExpiryDate { get; set; }
    }
}
