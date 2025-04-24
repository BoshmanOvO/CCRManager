using CommonContainerRegistry.Services.ServicesInterfaces;

namespace CommonContainerRegistry
{
    public class AppSettings
    {
        public string? RegistryName { get; set; } = string.Empty;
        public string? ClientId { get; set; } = string.Empty;
        public string? ClientSecret { get; set; } = string.Empty;
        public string? TenantId { get; set; } = string.Empty;
        public string? SubscriptionId { get; set; } = string.Empty;
        public string? ResourceGroupName { get; set; } = string.Empty;
    }
}
