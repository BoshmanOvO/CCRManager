namespace CommonContainerRegistry.Responses
{
    public class ScopeMapDetails
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreationDate { get; set; }
        public List<string>? Actions { get; set; }
    }
}
