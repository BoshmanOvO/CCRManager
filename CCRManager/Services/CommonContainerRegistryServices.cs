using CCRManager.Models;
using CCRManager.Responses;
using CCRManager.Services.Interfaces;
using CCRManager.Utils;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

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
            try
            {
                var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
                var tokenEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/tokens/{tokenName}?api-version=2023-01-01-preview";
                var request = new HttpRequestMessage(HttpMethod.Get, tokenEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);
                var response = await _httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"Token '{tokenName}' not found.");
                }
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get token '{tokenName}'. Status Code: {response.StatusCode}");
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                return new TokenDetails
                {
                    Name = root.GetProperty("name").GetString(),
                    CreationDate = root.GetProperty("properties").GetProperty("creationDate").GetDateTime(),
                    Status = root.GetProperty("properties").GetProperty("status").GetString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching token '{tokenName}': {ex.Message}");
            }
        }

        public async Task<ScopeMapDetails> GetScopeMapAsync(string scopeMapName)
        {
            if (_httpClient == null)
                throw new InvalidOperationException("HttpClient is not initialized.");

            if (_acrTokenProvider == null)
                throw new InvalidOperationException("AcrTokenProvider is not initialized.");

            try
            {
                var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
                var scopeMapEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/scopeMaps/{scopeMapName}?api-version=2023-01-01-preview";

                var request = new HttpRequestMessage(HttpMethod.Get, scopeMapEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"Scope map '{scopeMapName}' not found.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get scope map '{scopeMapName}'. Status Code: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                string? name = root.GetProperty("name").GetString();
                DateTime creationDate = root.GetProperty("properties").GetProperty("creationDate").GetDateTime();
                string? description = root.GetProperty("properties").TryGetProperty("description", out JsonElement descElement) ? descElement.GetString() : string.Empty;

                List<string> actions = [];
                if (root.GetProperty("properties").TryGetProperty("actions", out JsonElement actionsElement))
                {
                    foreach (var action in actionsElement.EnumerateArray())
                    {
                        if (action.ValueKind == JsonValueKind.String)
                            actions.Add(action.GetString());
                    }
                }

                return new ScopeMapDetails
                {
                    Name = name,
                    Description = description,
                    CreationDate = creationDate,
                    Actions = actions
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching scope map '{scopeMapName}': {ex.Message}");
            }
        }

        public async Task<ScopeMapDetails> CreateOrUpdateScopeMapAsync(ScopeMapRequest scopeMapRequest)
        {
            if (scopeMapRequest == null)
            {
                throw new ArgumentNullException(nameof(scopeMapRequest), "ScopeMapRequest cannot be null.");
            }
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Please check your configuration.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized. Please check your configuration.");
            }
            try
            {
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
                var responseContent = await response.Content.ReadAsStringAsync();
                ScopeMapDetails scopeMapDetails;
                try
                {
                    using JsonDocument doc = JsonDocument.Parse(responseContent);
                    var root = doc.RootElement;
                    string name = root.GetProperty("name").GetString()
                                  ?? throw new Exception("The response does not contain a valid 'name'.");
                    DateTime creationDate = root.GetProperty("properties").GetProperty("creationDate").GetDateTime();
                    string description = root.GetProperty("properties").GetProperty("description").GetString() ?? string.Empty;
                    var actions = root.GetProperty("properties").GetProperty("actions")
                                      .EnumerateArray()
                                      .Select(action => action.GetString() ?? string.Empty)
                                      .ToList();

                    scopeMapDetails = new ScopeMapDetails
                    {
                        Name = name,
                        CreationDate = creationDate,
                        Description = description,
                        Actions = actions
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to parse the response for ScopeMap '{scopeMapRequest.Name}': {ex.Message}", ex);
                }

                return scopeMapDetails;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating or updating ScopeMap '{scopeMapRequest.Name}': {ex.Message}", ex);
            }
        }

        public async Task<TokenOperationResult> GetOrCreateTokenAsync(TokenRequest tokenRequest)
        {
            bool tokenExists;
            try
            {
                await GetTokenAsync(tokenRequest.TokenName);
                tokenExists = true;
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    tokenExists = false;
                }
                else
                {
                    throw new HttpRequestException($"Error occurred while checking token existence for '{tokenRequest.TokenName}': {ex.Message}", ex);
                }
            }
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized. Please check your service configuration.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized. Please check your service configuration.");
            }
            try
            {
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
                    throw new HttpRequestException($"Failed to create or update token '{tokenRequest.TokenName}'. Status Code: {response.StatusCode}. Error: {errorContent}");
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                var prettyResponse = UtilityFunctions.PrettyPrintJson(responseContent);
                return new TokenOperationResult
                {
                    IsNewlyCreated = !tokenExists,
                    ResponseContent = prettyResponse
                };
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while processing token '{tokenRequest.TokenName}': {ex.Message}", ex);
            }
        }

        public async Task<string> CreateTokenPasswordAsync(PasswordRequest passwordRequest)
        {
            if (passwordRequest == null)
            {
                throw new ArgumentNullException(nameof(passwordRequest), "PasswordRequest payload cannot be null.");
            }
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized.");
            }
            try
            {
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
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to generate credentials for Token '{passwordRequest.TokenName}'. " +
                                                   $"Status Code: {response.StatusCode}. Error: {errorContent}");
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                if (!root.TryGetProperty("username", out JsonElement usernameElement) || usernameElement.ValueKind != JsonValueKind.String)
                {
                    throw new Exception("Username was not found in the response or is not a valid string.");
                }
                string username = usernameElement.GetString()!;
                if (!root.TryGetProperty("passwords", out JsonElement passwordsElement) || passwordsElement.ValueKind != JsonValueKind.Array)
                {
                    throw new Exception("Passwords array was not found in the response.");
                }
                JsonElement firstPassword = passwordsElement.EnumerateArray().FirstOrDefault();
                if (firstPassword.ValueKind == JsonValueKind.Undefined)
                {
                    throw new Exception("No passwords were returned in the response.");
                }
                var result = new
                {
                    username,
                    password = firstPassword
                };
                return UtilityFunctions.PrettyPrintJson(JsonSerializer.Serialize(result));
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while generating credentials for token '{passwordRequest.TokenName}': {ex.Message}", ex);
            }
        }

        public async Task<string> DeleteTokenAsync(string tokenName)
        {
            await GetTokenAsync(tokenName);
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
            var request = new HttpRequestMessage(HttpMethod.Delete, tokenEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);
            var response = await _httpClient.SendAsync(request);
            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Token \"{tokenName}\" not found. " +
                                               $"Status Code: {response.StatusCode}. Error: {errorContent}");
            }
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to delete token \"{tokenName}\". " +
                                               $"Status Code: {response.StatusCode}. Error: {errorContent}");
            }
            var resultContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(resultContent))
            {
                resultContent = $"Token \"{tokenName}\" deleted successfully.";
            }
            return resultContent;
        }

        public async Task<string> DeleteScopeMapAsync(string scopeMapName)
        {
            await GetScopeMapAsync(scopeMapName);
            if (_httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }
            if (_acrTokenProvider == null)
            {
                throw new InvalidOperationException("AcrTokenProvider is not initialized.");
            }
            var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
            var endpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/scopeMaps/{scopeMapName}?api-version=2023-01-01-preview";

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Scope map '{scopeMapName}' not found. Status Code: {response.StatusCode}. Error: {errorContent}");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete scope map '{scopeMapName}'. Status Code: {response.StatusCode}. Error: {errorContent}");
            }

            var resultContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(resultContent))
            {
                resultContent = $"Scope map '{scopeMapName}' deleted successfully.";
            }
            return resultContent;
        }

    }
}