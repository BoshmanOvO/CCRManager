using CommonContainerRegistry.Services.ServicesInterfaces;

namespace CommonContainerRegistry
{
    public class AppSettings
    {
        public string? RegistryName { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TenantId { get; set; }
        public string? SubscriptionId { get; set; }
        public string? ResourceGroupName { get; set; }
    }
}
