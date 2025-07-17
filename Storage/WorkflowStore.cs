using WorkflowEngine.Models;

namespace WorkflowEngine.Storage
{
    public static class WorkflowStore
    {
        public static Dictionary<string, WorkflowDefinition> Definitions = new();
        public static Dictionary<string, WorkflowInstance> Instances = new();
    }
}
