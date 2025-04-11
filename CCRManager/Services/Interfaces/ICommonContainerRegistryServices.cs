using CCRManager.Models;
using CCRManager.Responses;

namespace CCRManager.Services.Interfaces
{
    public interface ICommonContainerRegistryServices
    {
        Task<TokenDetails> GetTokenAsync(string tokenName); // to check if that particular token exists or not
        Task<string> CreateTokenPasswordAsync(PasswordRequest passwordRequest); // to create token's password
        Task<TokenOperationResult> GetOrCreateTokenAsync(TokenRequest tokeRequest); // to create token and update its scope map and status
        Task<ScopeMapDetails> CreateOrUpdateScopeMapAsync(ScopeMapRequest request); // only handled by admin
        Task<string> DeleteTokenAsync(string tokenName);
    }
}
