using CCRManager.Models;
using CCRManager.Responses;
using CCRManager.Services.Interfaces;
using CCRManager.Utils;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CCRManager.Services
{
    public class CommonContainerRegistryServices : ICommonContainerRegistryServices
    {
        private readonly IConfiguration _config;
        private readonly HttpClient? _httpClient;
        private readonly IAcrTokenProvider? _acrTokenProvider;
        private readonly string? _registryName;
        private readonly string? _subscriptionId;
        private readonly string? _resourceGroupName;
        public CommonContainerRegistryServices(IConfiguration config, HttpClient httpClient, IAcrTokenProvider acrTokenProvider)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _acrTokenProvider = acrTokenProvider ?? throw new ArgumentNullException(nameof(acrTokenProvider));
            _registryName = _config["Azure:RegistryName"] ?? throw new ArgumentNullException(nameof(config), "RegistryName cannot be null");
            _subscriptionId = _config["Azure:SubscriptionId"] ?? throw new ArgumentNullException(nameof(config), "Subscription cannot be null");
            _resourceGroupName = _config["Azure:ResourceGroupName"] ?? throw new ArgumentNullException(nameof(config), "ResourceGroupName cannot be null");
        }

        public async Task<TokenDetails> GetTokenAsync(string tokenName)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized.");
            }
            var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
            var tokenEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/tokens/{tokenName}?api-version=2023-01-01-preview";
            var request = new HttpRequestMessage(HttpMethod.Get, tokenEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);
            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Token \"{tokenName}\" not found. " + 
                                               $"Status Code: {response.StatusCode}. Error: {errorContent}");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            using (JsonDocument doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                var name = root.GetProperty("name").GetString();
                var creationDate = root.GetProperty("properties").GetProperty("creationDate").GetDateTime();
                var status = root.GetProperty("properties").GetProperty("status").GetString();
                return new TokenDetails
                {
                    Name = name,
                    CreationDate = creationDate,
                    Status = status
                };
            }
        }

        public async Task<ScopeMapDetails> CreateOrUpdateScopeMapAsync(ScopeMapRequest scopeMapRequest)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized.");
            }
            var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
            var tokenEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/scopeMaps/{scopeMapRequest.Name}?api-version=2023-01-01-preview";
            var requestBody = new
            {
                properties = new
                {
                    description = scopeMapRequest.Description,
                    actions = scopeMapRequest.Permissions.Select(permission =>
                                $"repositories/{_registryName}/{permission.ToString().ToLower()}").ToArray()
                }
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, tokenEndpoint)
            {
                Content = jsonContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to create or update ScopeMap '{scopeMapRequest.Name}'. " +
                                               $"Status Code: {response.StatusCode}. Error: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            using (JsonDocument doc = JsonDocument.Parse(responseContent))
            {
                var root = doc.RootElement;
                var name = root.GetProperty("name").GetString();
                var creationDate = root.GetProperty("properties").GetProperty("creationDate").GetDateTime();
                var description = root.GetProperty("properties").GetProperty("description").GetString();
                List<string> actions = root.GetProperty("properties").GetProperty("actions").EnumerateArray()
                    .Select(action => action.GetString() ?? string.Empty).ToList();
                return new ScopeMapDetails
                {
                    Name = name,
                    CreationDate = creationDate,
                    Description = description,
                    Actions = actions
                };
            }
        }

        public async Task<string> GetOrCreateTokenAsync(TokenRequest tokenRequest)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized.");
            }
            var accessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
            var tokenEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/tokens/{tokenRequest.TokenName}?api-version=2023-01-01-preview";
            var requestBody = new
            {
                properties = new
                {
                    scopeMapId = $"/subscriptions/{_subscriptionId}/resourceGroups/MyResource/providers/Microsoft.ContainerRegistry/registries/{_registryName}/scopeMaps/{tokenRequest.ScopeMapName}",
                    tokenRequest.Status,
                    credentials = new
                    {
                        passwords = Array.Empty<object>(),
                    }
                }
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, tokenEndpoint)
            {
                Content = jsonContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to create or update Token '{tokenRequest.TokenName}'. " +
                                               $"Status Code: {response.StatusCode}. Error: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return UtilityFunctions.PrettyPrintJson(responseContent);
        }

        public async Task<string> CreateTokenPasswordAsync(PasswordRequest passwordRequest)
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized.");
            }
            var accessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
            var tokenEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/generateCredentials?api-version=2023-01-01-preview";
            var requestBody = new
            {
                tokenId = $"/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/tokens/{passwordRequest.TokenName}",
                expiry = UtilityFunctions.ConvertToIso8601(passwordRequest.PasswordExpiryDate)
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
            {
                Content = jsonContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to generate credentials for Token '{passwordRequest.TokenName}'. " +
                                               $"Status Code: {response.StatusCode}. Error: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return UtilityFunctions.PrettyPrintJson(responseContent);
        }
    }
}