namespace CCRManager.Models
{
    public class TokenRequest
    {
        public required string TokenName { get; set; }
        public required string ScopeMapName { get; set; }
        public required string TokenPasswordExpiryDateAndTime { get; set; }
    }
}
