using Refit;

namespace CommonContainerRegistry.Services.ServicesInterfaces
{
    public interface IAzureAuthClient
    {
        [Post("/{tenantId}/oauth2/token")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<string> GetAcrAccessTokenAsync([AliasAs("tenantId")] string tenantId, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> form);
    }
}
