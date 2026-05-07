# AGENTS.md

## Project overview

- This repository is a small .NET 10 solution of Azure AI evaluation experiments, not a layered application. The solution currently contains three independent projects: `Eval.Agent`, `Eval.FoundryAgent`, and `Eval.BuiltIn.Coherence`.
- The common pattern is: create an `AIProjectClient` with `DefaultAzureCredential`, read required settings from environment variables, then call Azure-hosted agent/evaluation APIs directly.
- There is no shared library yet; duplication across the console apps is intentional and reflects separate evaluation scenarios.

## Workspace map

- `EvaluationSLN.slnx` includes all three projects. Use `dotnet sln EvaluationSLN.slnx list` if the IDE view looks incomplete.
- `Eval.Agent/Program.cs` runs a simple Microsoft Agents Framework-backed prompt against a model deployment via `AsAIAgent(...)`.
- `Eval.FoundryAgent/Program.cs` retrieves an existing Foundry agent by name, then runs a prompt against that remote agent.
- `Eval.BuiltIn.Coherence/TestCoherenceInline.cs` is the only test project; it creates a live evaluation, starts a run with inline JSONL-style payload data, polls until completion, then prints result items.

## Required environment and auth

- `Eval.Agent` requires `ENDPOINT` and `DEPLOYMENT_NAME`.
- `Eval.FoundryAgent` requires `ENDPOINT` and `AGENT_NAME`.
- `Eval.BuiltIn.Coherence` requires at least `ENDPOINT` and `DEPLOYMENT_NAME`.
- All Azure calls use `DefaultAzureCredential`; local runs/tests assume you are already authenticated in a credential source that `Azure.Identity` can see.
- The solution targets `net10.0`; builds currently emit `NETSDK1057`, so expect to need a preview .NET SDK.

## Verified commands

- Restore/build the full workspace: `dotnet build EvaluationSLN.slnx`
- Run the simple deployment-backed agent: `dotnet run --project .\Eval.Agent\Eval.Agent.csproj`
- Run the remote Foundry agent sample: `dotnet run --project .\Eval.FoundryAgent\Eval.FoundryAgent.csproj`
- Run the live coherence evaluation test: `dotnet test .\Eval.BuiltIn.Coherence\Eval.BuiltIn.Coherence.csproj`
- Note: `dotnet build EvaluationSLN.slnx` succeeds in the current workspace; `dotnet run` / `dotnet test` fail fast without the required environment variables and reachable Azure resources.

## Code patterns to preserve

- These projects use top-level statements instead of explicit `Program` classes; keep new console experiments consistent unless there is a strong reason to refactor.
- Console UX is done with `Spectre.Console` (`AnsiConsole.MarkupLine`, `AnsiConsole.Ask`, `AnsiConsole.WriteLine`) rather than raw `Console.WriteLine` in the apps.
- Fail-fast environment validation is the norm: see `ArgumentException.ThrowIfNullOrEmpty(...)` near the top of both `Program.cs` files.
- Azure service access is intentionally direct and local to each project. For example, `Eval.FoundryAgent/Program.cs` creates `AIProjectClient`, then uses `AgentAdministrationClient.GetAgentAsync(...)`, then `AsAIAgent(...)`.
- `Eval.BuiltIn.Coherence/TestCoherenceInline.cs` parses low-level `ClientResult` payloads with `Utf8JsonReader`/`JsonDocument` helpers (`ParseClientResult`, `GetResultsCounts`, `GetResultsListAsync`). Reuse those helpers or that style when the SDK does not expose strongly typed evaluation result models.

## Testing and change guidance

- Treat `Eval.BuiltIn.Coherence` as a live integration test, not an isolated unit test. Changes there may create remote evaluations and require polling.
- When editing evaluation payloads, keep the inline dataset shape aligned with `GetDataConfig(...)` (`query`, `response`, `context`, `ground_truth`).
- If you add another experiment, prefer a new sibling project in the solution over mixing multiple scenarios into one `Program.cs`.
