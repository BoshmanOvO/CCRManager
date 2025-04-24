using CommonContainerRegistry.Models.Requests;
using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Responses
{
    public class TokenPasswordResponse
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("passwords")]
        public List<PasswordInfo> Passwords { get; set; } = [];
    }
}
