using CommonContainerRegistry.Models.Requests;
using CommonContainerRegistry.Models.Responses;

namespace CommonContainerRegistry.Services.ServicesInterfaces
{
    public interface ICommonContainerRegistryServices
    {
        Task<TokenDetails> GetTokenAsync(string tokenName); // to check if that particular token exists or not
        Task<string> CreateTokenPasswordAsync(PasswordRequest passwordRequest); // to create token's password
        Task<TokenOperationResult> CreateOrUpdateTokenAsync(TokenRequest tokeRequest); // to create token and update its scope map and status
        Task<ScopeMapOperationResult> CreateOrUpdateScopeMapAsync(ScopeMapRequest request); // only handled by admin
        Task<string> DeleteTokenAsync(string tokenName);
        Task<string> DeleteScopeMapAsync(string scopeMapName);
        Task<ScopeMapDetails> GetScopeMapAsync(string scopeMapName);
    }
}
