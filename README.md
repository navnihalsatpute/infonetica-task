# Configurable Workflow Engine (State Machine API)

This is a simple backend project built using .NET 8 and C# for the Infonetica software engineering internship take-home assignment.

It allows you to:
- Create workflows (a flow of steps)
- Start instances (a running copy of that workflow)
- Move between steps using actions (transitions)
- View the current step and the history of actions

## How to Run
Make sure .NET 8 is installed. Then:
```bash
cd WorkflowEngine
dotnet run
```
The API will start at:
http://localhost:5000

## API Endpoints
These are the actions you can do:

- POST /workflow/definition
Add a new workflow (with steps and actions)

- GET /workflow/definition/{id}
See a saved workflow

- POST /workflow/instance/{definitionId}
Start a new instance of a workflow

- POST /workflow/instance/{id}/execute/{actionId}
Move the instance to the next step (if allowed)

- GET /workflow/instance/{id}
Check current step and history

## The API makes sure that:

A workflow has exactly one starting step

No steps or actions have duplicate IDs

You can't move if the action or target step is disabled

You can't act once the final step is reached

Only valid actions from current step are allowed

# Notes
- This project uses in-memory storage, which means all data will reset if the app restarts.

- It does not use any external database or frameworks — only built-in .NET 8 and C# features.
