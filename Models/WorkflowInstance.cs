namespace WorkflowEngine.Models
{
    public class WorkflowInstance
    {
        public string Id { get; set; }
        public string DefinitionId { get; set; }
        public string CurrentState { get; set; }
        public List<(string ActionId, DateTime Timestamp)> History { get; set; } = new();
    }
}
