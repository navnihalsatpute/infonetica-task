using WorkflowEngine.Models;
using WorkflowEngine.Storage;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Create a workflow definition
app.MapPost("/workflow/definition", (WorkflowDefinition def) =>
{
    if (WorkflowStore.Definitions.ContainsKey(def.Id))
        return Results.BadRequest("Definition ID already exists.");

    if (def.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Workflow must have exactly one initial state.");

    WorkflowStore.Definitions[def.Id] = def;
    return Results.Ok(def);
});

// Get a workflow definition
app.MapGet("/workflow/definition/{id}", (string id) =>
{
    return WorkflowStore.Definitions.TryGetValue(id, out var def) ? Results.Ok(def) : Results.NotFound();
});

// Start a new instance
app.MapPost("/workflow/instance/{definitionId}", (string definitionId) =>
{
    if (!WorkflowStore.Definitions.TryGetValue(definitionId, out var def))
        return Results.NotFound("Definition not found.");

    var initial = def.States.First(s => s.IsInitial);
    var instance = new WorkflowInstance
    {
        Id = Guid.NewGuid().ToString(),
        DefinitionId = definitionId,
        CurrentState = initial.Id,
        History = new() { ("INIT", DateTime.UtcNow) }
    };

    WorkflowStore.Instances[instance.Id] = instance;
    return Results.Ok(instance);
});

// Execute an action
app.MapPost("/workflow/instance/{id}/execute/{actionId}", (string id, string actionId) =>
{
    if (!WorkflowStore.Instances.TryGetValue(id, out var instance))
        return Results.NotFound("Instance not found.");

    if (!WorkflowStore.Definitions.TryGetValue(instance.DefinitionId, out var def))
        return Results.NotFound("Definition not found.");

    var action = def.Actions.FirstOrDefault(a => a.Id == actionId);
    if (action == null || !action.Enabled)
        return Results.BadRequest("Invalid or disabled action.");

    if (!action.FromStates.Contains(instance.CurrentState))
        return Results.BadRequest("Action cannot be executed from current state.");

    var toState = def.States.FirstOrDefault(s => s.Id == action.ToState && s.Enabled);
    if (toState == null)
        return Results.BadRequest("Target state is invalid or disabled.");

    if (def.States.First(s => s.Id == instance.CurrentState).IsFinal)
        return Results.BadRequest("Cannot act on final state.");

    instance.CurrentState = action.ToState;
    instance.History.Add((actionId, DateTime.UtcNow));
    return Results.Ok(instance);
});

// Get instance state and history
app.MapGet("/workflow/instance/{id}", (string id) =>
{
    return WorkflowStore.Instances.TryGetValue(id, out var inst) ? Results.Ok(inst) : Results.NotFound();
});

app.MapGet("/", () => "Workflow API is running.");

app.Run();
