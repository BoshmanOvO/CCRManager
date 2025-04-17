namespace CommonContainerRegistry.Models
{
    public class PasswordRequest
    {
        public required string TokenName {  get; set; }
        public required string PasswordExpiryDate { get; set; }
    }
}
