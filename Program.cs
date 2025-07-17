using WorkflowEngine.Models;
using WorkflowEngine.Storage;

var app = WebApplication.CreateBuilder(args).Build();

app.MapPost("/workflow/definition", (WorkflowDefinition def) =>
    !def.IsValid(out var error) ? Results.BadRequest(error) :
    !WorkflowStore.Definitions.TryAdd(def.Id, def) ? Results.Conflict("Workflow ID already exists.") :
    Results.Created($"/workflow/definition/{def.Id}", def));

app.MapGet("/workflow/definition/{id}", (string id) =>
    WorkflowStore.Definitions.TryGetValue(id, out var def) ? Results.Ok(def) : Results.NotFound());

app.MapPost("/workflow/instance/{definitionId}", (string definitionId) =>
{
    if (!WorkflowStore.Definitions.TryGetValue(definitionId, out var def))
        return Results.NotFound("Definition not found.");

    var init = def.States.First(s => s.IsInitial);
    var instance = new WorkflowInstance { DefinitionId = definitionId, CurrentState = init.Id };
    instance.History.Add(("INIT", DateTime.UtcNow));
    WorkflowStore.Instances[instance.Id] = instance;
    return Results.Created($"/workflow/instance/{instance.Id}", instance);
});

app.MapPost("/workflow/instance/{id}/execute/{actionId}", (string id, string actionId) =>
{
    if (!WorkflowStore.Instances.TryGetValue(id, out var inst)) return Results.NotFound("Instance not found.");
    if (!WorkflowStore.Definitions.TryGetValue(inst.DefinitionId, out var def)) return Results.NotFound("Definition not found.");
    if (def.States.First(s => s.Id == inst.CurrentState).IsFinal) return Results.BadRequest("Already at final state.");

    if (def.Actions.FirstOrDefault(a => a.Id == actionId && a.Enabled && a.FromStates.Contains(inst.CurrentState)) is not { } action)
        return Results.BadRequest("Invalid action for current state.");

    if (def.States.FirstOrDefault(s => s.Id == action.ToState && s.Enabled) is not { } next)
        return Results.BadRequest("Invalid or disabled target state.");

    inst.CurrentState = next.Id;
    inst.History.Add((actionId, DateTime.UtcNow));
    return Results.Ok(inst);
});

app.MapGet("/workflow/instance/{id}", (string id) =>
    WorkflowStore.Instances.TryGetValue(id, out var inst) ? Results.Ok(inst) : Results.NotFound());

app.Run();