namespace CCRManager.Models
{
    public class ScopeMapRequest
    {
        public required string ScopeMapName { get; set; }
        public required string ScopeMapDescription { get; set; }
        public required List<string> Actions { get; set; }
    }
}
