# Source Code

This folder contains all demo source code for the AI evaluation options project.

| Folder | Description |
|---|---|
| [`EvaluationSLN/`](EvaluationSLN/) | .NET 10 solution containing all evaluation demos and shared test base |

## .NET Requirement

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Solution Projects

`src/EvaluationSLN/` currently contains:

- `Eval.Agent`
- `Eval.FoundryAgent`
- `Eval.Agent.AIAssistedEvaluation`
- `Eval.BuiltIn.AgentEvaluation`
- `Eval.BuiltIn.BleuScoreTests`
- `Eval.BuiltIn.Coherence`
- `Eval.BuiltIn.EvaluatorsCatalog`
- `Eval.BuiltIn.ViolenceDetection`
- `Eval.Tests.Base`

## Getting Started

Before running any demos, ensure you have completed the project setup:

```bash
bash ../scripts/setup.sh
```

See the [project README](../README.md) for full prerequisites and setup instructions.
