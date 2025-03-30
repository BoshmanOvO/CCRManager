using CCRManager.Models;

namespace CCRManager.Services.Interfaces
{
    public interface ICommonContainerRegistryServices
    {
        Task<string> GetTokenAsync(string tokenName); // to check if that particular token exists or not
        Task<string> CreateTokenPasswordAsync(string tokenName, long tokenExpiryDate); // to create token's password
        Task<string> GetOrCreateTokenAsync(string tokenName, long tokenExpiryDate, string scopeMapName, string status); // to create token and update its scope map
        Task<string> CreateOrUpdateScopeMapAsync(ScopeMapRequest request); // only handled by admin
    }
}
