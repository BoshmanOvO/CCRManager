using CCRManager.Services.Interfaces;
using System.Net.Http;
using System.Text.Json;

namespace CCRManager.Services
{
    public class AcrTokenProvider : IAcrTokenProvider
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AcrTokenProvider(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }
        public async Task<string> GetAcrAccessTokenAsync()
        {
            try
            {
                string _registryUrl = _config["Azure:RegistryName"] ?? throw new ArgumentNullException(nameof(_registryUrl), "RegistryName cannot be null");
                string _clientId = _config["Azure:ClientId"] ?? throw new ArgumentNullException(nameof(_clientId), "ClientId cannot be null");
                string _clientSecret = _config["Azure:ClientSecret"] ?? throw new ArgumentNullException(nameof(_clientSecret), "ClientSecret cannot be null");
                string _tenantId = _config["Azure:TenantId"] ?? throw new ArgumentNullException(nameof(_tenantId), "TenantId cannot be null");

                var tokenUrl = $"https://login.microsoftonline.com/{_tenantId}/oauth2/token";
                var requestBody = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "resource", $"https://management.azure.com/" }
                };

                var requestContent = new FormUrlEncodedContent(requestBody);
                var response = await _httpClient.PostAsync(tokenUrl, requestContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                if (tokenResponse != null && tokenResponse.TryGetValue("access_token", out var accessToken))
                {
                    return accessToken;
                }
                else
                {
                    throw new ApplicationException("Failed to get ACR access token: access_token not found in response.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get ACR access token", ex);
            }
        }
    }
}
