namespace CCRManager.Services.Interfaces
{
    public interface IAcrTokenProvider
    {
        // Retrieves an Azure Container Registry (ACR) access token using client credentials.
        // A string representing the access token.
        Task<string> GetAcrAccessTokenAsync();
    }
}
