﻿using System.Text.Json.Serialization;

namespace CommonContainerRegistry.Models.Responses
{
    public class TokenDetails
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("properties")]
        public TokenProperties? Properties { get; set; }
    }
}
