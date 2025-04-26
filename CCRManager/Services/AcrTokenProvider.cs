using CommonContainerRegistry.Constants;
using CommonContainerRegistry.Services.ServicesInterfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CommonContainerRegistry.Services
{
    public class AcrTokenProvider(IOptions<AppSettings> appSettings, IAzureAuthClient azureAuthClient) : IAcrTokenProvider
    {
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly IAzureAuthClient _azureAuthClient = azureAuthClient;

        public async Task<string> GetAcrAccessTokenAsync()
        {
            string _tenantId = _appSettings.TenantId ?? throw new ArgumentNullException(nameof(_tenantId), "TenantId cannot be null");
            try
            {
                var requestBody = new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _appSettings.ClientId!,
                    ["client_secret"] = _appSettings.ClientSecret!,
                    ["resource"] = "https://management.azure.com/"
                };
                var result = await _azureAuthClient.GetAcrAccessTokenAsync(_tenantId, requestBody) ?? null;
                using var doc = JsonDocument.Parse(result);
                if (!doc.RootElement.TryGetProperty("access_token", out var tok) ||
                    tok.ValueKind != JsonValueKind.String)
                {
                    throw new ApplicationException("Azure AD response did not contain an access_token.");
                }
                return tok.GetString()!;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get ACR access token", ex);
            }
        }
    }
}
