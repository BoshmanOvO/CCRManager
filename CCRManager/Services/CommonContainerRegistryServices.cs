using CommonContainerRegistry.Utils;
using CommonContainerRegistry.Services.ServicesInterfaces;
using System.Text.Json;
using CommonContainerRegistry.Constants;
using CommonContainerRegistry.Models.Requests;
using CommonContainerRegistry.Models.Responses;
using Microsoft.Extensions.Options;
using CommonContainerRegistry.Models.Payloads;
using Microsoft.AspNetCore.Http.HttpResults;


namespace CommonContainerRegistry.Services
{
    public class CommonContainerRegistryServices(IAzureApiService azureApiClient, IAcrTokenProvider acrTokenProvider, IOptions<AppSettings> appSettings) : ICommonContainerRegistryServices
    {

        private readonly IAzureApiService _azureApiService = azureApiClient;
        private readonly IAcrTokenProvider _acrTokenProvider = acrTokenProvider;
        private readonly AppSettings _appSettings = appSettings.Value;

        public async Task<TokenDetails> GetTokenAsync(string tokenName)
        {
            try
            {
                var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
                var authorizationHeader = $"Bearer {acrAccessToken}";

                var response = await _azureApiService.GetTokenAsync(
                    _appSettings.SubscriptionId,
                    _appSettings.ResourceGroupName,
                    _appSettings.RegistryName,
                    tokenName,
                    authorizationHeader
                ) ?? null;
                var tokenDetails = new TokenDetails
                {
                    Name = response?.Name,
                    Properties = new TokenProperties
                    {
                        CreationDate = response?.Properties?.CreationDate,
                        Status = response?.Properties?.Status
                    }
                };
                return tokenDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ScopeMapDetails> GetScopeMapAsync(string scopeMapName)
        {
            try
            {
                var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
                var authorizationHeader = $"Bearer {acrAccessToken}";
                var response = await _azureApiService.GetScopeMapAsync(
                    _appSettings.SubscriptionId,
                    _appSettings.ResourceGroupName,
                    _appSettings.RegistryName,
                    scopeMapName,
                    authorizationHeader
                ) ?? null;

                return new ScopeMapDetails
                {
                    Name = response?.Name,
                    Properties = new ScopeMapProperties
                    {
                        CreationDate = response?.Properties?.CreationDate ?? default,
                        Description = response?.Properties?.Description ?? string.Empty,
                        Permissions = response?.Properties?.Permissions ?? []
                    },
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching scope map '{scopeMapName}': {ex.Message}");
            }
        }

        public async Task<ScopeMapOperationResult> CreateOrUpdateScopeMapAsync(ScopeMapRequest scopeMapRequest)
        {
            bool scopeMapExists = await ScopeMapExistAsync(scopeMapRequest.Name);
            try
            {
                var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
                var authorizationHeader = $"Bearer {acrAccessToken}";
                var payload = new ScopeMapRequestPayload
                {
                    Properties = new ScopeMapPropertiesPayload
                    {
                        Description = scopeMapRequest.Description,
                        Permissions = scopeMapRequest.Permissions?.Select(p => $"repositories/{_appSettings.RegistryName}/{p.ToString().ToLower()}").ToList(),
                    }
                };
                var response = await _azureApiService.CreateOrUpdateScopeMapAsync(
                    _appSettings.SubscriptionId,
                    _appSettings.ResourceGroupName,
                    _appSettings.RegistryName,
                    scopeMapRequest.Name,
                    payload,
                    authorizationHeader
                ) ?? null;
                var scopeMapDetails = new ScopeMapDetails
                {
                    Name = response?.Name,
                    Properties = new ScopeMapProperties
                    {
                        CreationDate = response?.Properties?.CreationDate ?? default,
                        Description = response?.Properties?.Description ?? string.Empty,
                        Permissions = response?.Properties?.Permissions ?? []
                    }
                };
                return new ScopeMapOperationResult
                {
                    IsNewlyCreated = !scopeMapExists,
                    ScopeMapDetails = scopeMapDetails
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating or updating ScopeMap '{scopeMapRequest.Name}': {ex.Message}", ex);
            }
        }

        public async Task<TokenOperationResult> CreateOrUpdateTokenAsync(TokenRequest tokenRequest)
        {
            bool tokenExists = await TokenExistsAsync(tokenRequest.TokenName);
            try
            {
                var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
                var authorizationHeader = $"Bearer {acrAccessToken}";
                var payload = new TokenRequestPayload
                {
                    TokenProperties = new TokenPropertiesPayload
                    {
                        ScopeMapId = string.Format(AzureApiEndpoints.ScopeMapId, _appSettings.SubscriptionId, _appSettings.ResourceGroupName, _appSettings.RegistryName, tokenRequest.ScopeMapName),
                        Status = tokenRequest.Status,
                        TokenCredentials = new TokenCredentialsPayload
                        {
                            Passwords = Array.Empty<object>()
                        }
                    }
                };
                var response = await _azureApiService.CreateOrUpdateTokenAsync(
                    _appSettings.SubscriptionId,
                    _appSettings.ResourceGroupName,
                    _appSettings.RegistryName,
                    tokenRequest.TokenName,
                    payload,
                    authorizationHeader
                ) ?? null;
                return new TokenOperationResult
                {
                    IsNewlyCreated = !tokenExists,
                    ResponseContent = response?.ResponseContent
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while processing token '{tokenRequest.TokenName}': {ex.Message}", ex);
            }
        }

        public async Task<string> CreateTokenPasswordAsync(PasswordRequest passwordRequest)
        {
            try
            {
                var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
                var authorizationHeader = $"Bearer {acrAccessToken}";
                var tokenId = string.Format(AzureApiEndpoints.TokenId, _appSettings.SubscriptionId, _appSettings.ResourceGroupName, _appSettings.RegistryName, passwordRequest.TokenName);
                var payload = new TokenPasswordRequestPayload
                {
                    TokenId = tokenId,
                    ExpiryDate = UtilityFunctions.ConvertToIso8601(passwordRequest.PasswordExpiryDate)
                };
                var response = await _azureApiService.CreateTokenPasswordAsync(
                    _appSettings.SubscriptionId,
                    _appSettings.ResourceGroupName,
                    _appSettings.RegistryName,
                    payload,
                    authorizationHeader
                ) ?? null;
                var firstPassword = response?.Passwords.FirstOrDefault();
                var result = new TokenPasswordResponse
                {
                    Username = response.Username,
                    Passwords =
                    [
                        new() {
                            Name = firstPassword.Name,
                            Value = firstPassword.Value,
                            Expiry = firstPassword.Expiry,
                        }
                    ]
                };
                return UtilityFunctions.PrettyPrintJson(JsonSerializer.Serialize(result));
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while generating credentials for token '{passwordRequest.TokenName}': {ex.Message}", ex);
            }
        }

        public async Task<string> DeleteTokenAsync(string tokenName)
        {
            await GetTokenAsync(tokenName);
            var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
            var authorizationHeader = $"Bearer {acrAccessToken}";
            var _ = await _azureApiService.DeleteTokenAsync(
                _appSettings.SubscriptionId,
                _appSettings.ResourceGroupName,
                _appSettings.RegistryName,
                tokenName,
                authorizationHeader
            ) ?? null;
            return $"Token '{tokenName}' deleted successfully.";
        }

        public async Task<string> DeleteScopeMapAsync(string scopeMapName)
        {
            await GetScopeMapAsync(scopeMapName);
            var acrAccessToken = await _acrTokenProvider.GetAcrAccessTokenAsync();
            var authorizationHeader = $"Bearer {acrAccessToken}";
            var _ = await _azureApiService.DeleteScopeMapAsync(
                _appSettings.SubscriptionId,
                _appSettings.ResourceGroupName,
                _appSettings.RegistryName,
                scopeMapName,
                authorizationHeader
            ) ?? null;
            return $"Scope map '{scopeMapName}' deleted successfully.";
        }


        public async Task<bool> TokenExistsAsync(string tokenName)
        {
            try
            {
                var token = await GetTokenAsync(tokenName);
                return token != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ScopeMapExistAsync(string scopeMapName)
        {
            try
            {
                var scopeMap = await GetScopeMapAsync(scopeMapName);
                return scopeMap != null;
            }
            catch
            {
                return false;
            }
        }


    }
}