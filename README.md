# Configurable Workflow Engine (State Machine API)

## How to Run
```bash
cd WorkflowEngine
dotnet run
```

## Features
- Define workflows with states and actions
- Start workflow instances
- Transition between states using valid actions
- In-memory storage (no DB needed)

## Assumptions
- IDs for definitions, states, and actions must be unique
- Each workflow must have exactly one initial state
- Final states block further actions
- History is kept as a list of (actionId, timestamp)

## API Endpoints
- POST /workflow/definition
- GET /workflow/definition/{id}
- POST /workflow/instance/{definitionId}
- POST /workflow/instance/{id}/execute/{actionId}
- GET /workflow/instance/{id}
