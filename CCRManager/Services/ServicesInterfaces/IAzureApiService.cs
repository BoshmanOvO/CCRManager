using CommonContainerRegistry.Models.Payloads;
using CommonContainerRegistry.Models.Requests;
using CommonContainerRegistry.Models.Responses;
using Refit;

namespace CommonContainerRegistry.Services.ServicesInterfaces
{
    public interface IAzureApiService
    {
        [Get("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{registryName}/tokens/{tokenName}?api-version=2023-01-01-preview")]
        Task<TokenDetails> GetTokenAsync(string? subscriptionId, string? resourceGroupName, string? registryName, string? tokenName, [Header("Authorization")] string? authorizationHeader);

        [Get("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{registryName}/scopeMaps/{scopeMapName}?api-version=2023-01-01-preview")]
        Task<ScopeMapDetails> GetScopeMapAsync(string? subscriptionId, string? resourceGroupName, string? registryName, string? scopeMapName, [Header("Authorization")] string? authorizationHeader);

        [Put("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{registryName}/scopeMaps/{scopeMapName}?api-version=2023-01-01-preview")]
        Task<ScopeMapResponse> CreateOrUpdateScopeMapAsync(string? subscriptionId, string? resourceGroupName, string? registryName, string? scopeMapName, [Body] ScopeMapRequestPayload body, [Header("Authorization")] string authorizationHeader);

        [Put("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{registryName}/tokens/{tokenName}?api-version=2023-01-01-preview")]
        Task<TokenOperationResult> CreateOrUpdateTokenAsync(string? subscriptionId, string? resourceGroupName, string? registryName, string? tokenName, [Body] TokenRequestPayload body, [Header("Authorization")] string authorizationHeader);

        [Delete("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{registryName}/tokens/{tokenName}?api-version=2023-01-01-preview")]
        Task<string> DeleteTokenAsync(string? subscriptionId, string? resourceGroupName, string? registryName, string? tokenName, [Header("Authorization")] string authorizationHeader);

        [Delete("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{registryName}/scopeMaps/{scopeMapName}?api-version=2023-01-01-preview")]
        Task<string> DeleteScopeMapAsync(string? subscriptionId, string? resourceGroupName, string? registryName, string? scopeMapName, [Header("Authorization")] string authorizationHeader);

        [Post("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ContainerRegistry/registries/{registryName}/generateCredentials?api-version=2023-01-01-preview")]
        Task<TokenPasswordResponse> CreateTokenPasswordAsync(string? subscriptionId, string? resourceGroupName, string? registryName, [Body] TokenPasswordRequestPayload body, [Header("Authorization")] string authorizationHeader);
    }
}



// Api version should be dynamic.
