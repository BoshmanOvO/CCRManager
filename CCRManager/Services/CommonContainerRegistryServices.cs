using CCRManager.Models;
using CCRManager.Services.Interfaces;
using CCRManager.Utils;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public async Task<string> GetTokenAsync(string tokenName)
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
                throw new HttpRequestException($"Token {tokenName} not found");
            }
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var prettyJson = UtilityFunctions.PrettyPrintJson(responseContent);
            return prettyJson;
        }

        public async Task<string> CreateOrUpdateScopeMapAsync(ScopeMapRequest scopeMapRequest)
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
            var tokenEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/scopeMaps/{scopeMapRequest.ScopeMapName}?api-version=2023-01-01-preview";
            var requestBody = new
            {
                properties = new
                {
                    description = scopeMapRequest.ScopeMapDescription,
                    actions = scopeMapRequest.Actions
                }
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, tokenEndpoint)
            {
                Content = jsonContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", acrAccessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent =  await response.Content.ReadAsStringAsync();
            string prettyJson = UtilityFunctions.PrettyPrintJson(responseContent);
            return prettyJson;
        }

        public async Task<string> GetOrCreateTokenAsync(string tokenName, long epochMilliseconds, string scopeMapName, string status)
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
            var tokenEndpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/tokens/{tokenName}?api-version=2023-01-01-preview";
            var requestBody = new
            {
                properties = new
                {
                    scopeMapId = $"/subscriptions/{_subscriptionId}/resourceGroups/MyResource/providers/Microsoft.ContainerRegistry/registries/{_registryName}/scopeMaps/{scopeMapName}",
                    status,
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
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var prettyJson = UtilityFunctions.PrettyPrintJson(responseContent);
            return prettyJson;
        }

        public async Task<string> CreateTokenPasswordAsync(string tokenName, long epochMilliseconds)
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
            var endpoint = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/generateCredentials?api-version=2023-01-01-preview";
            var requestBodyObject = new
            {
                tokenId = $"/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{_registryName}/tokens/{tokenName}",
                expiry = UtilityFunctions.ConvertDateTimeStringToIso8601(epochMilliseconds)
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBodyObject), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = jsonContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            string prettyJson = UtilityFunctions.PrettyPrintJson(responseContent);
            return prettyJson;
        }

    }
}