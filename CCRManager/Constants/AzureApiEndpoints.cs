namespace CommonContainerRegistry.Constants
{
    public class AzureApiEndpoints
    {
        public const string GetToken = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/tokens/{3}?api-version=2023-01-01-preview";
        public const string GetScopeMap = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/scopeMaps/{3}?api-version=2023-01-01-preview";
        public const string CreateScopeMapEndPoint = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/scopeMaps/{3}?api-version=2023-01-01-preview";
        public const string CreateTokenEndpoint = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/tokens/{3}?api-version=2023-01-01-preview";
        public const string CreateTokenPasswordEndpoint = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/generateCredentials?api-version=2023-01-01-preview";
        public const string DeleteTokenEndpoint = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/tokens/{3}?api-version=2023-01-01-preview";
        public const string DeleteScopeMapEndpoint = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/scopeMaps/{3}?api-version=2023-01-01-preview";
        public const string TokenId = "/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/tokens/{3}";
        public const string ScopeMapId = "/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerRegistry/registries/{2}/scopeMaps/{3}";
    }
}
