# Tests

This folder contains test documentation for the evaluation demos in this repository.

## Structure

```
tests/
└── README.md

src/EvaluationSLN/
├── Eval.Agent.AIAssistedEvaluation/      # Agent framework AI-assisted evaluation tests
├── Eval.BuiltIn.AgentEvaluation/         # Built-in agent evaluation tests
├── Eval.BuiltIn.BleuScoreTests/          # BLEU score evaluator tests
├── Eval.BuiltIn.Coherence/               # Coherence evaluator tests
├── Eval.BuiltIn.EvaluatorsCatalog/       # Evaluator catalog tests
└── Eval.BuiltIn.ViolenceDetection/       # Violence detection evaluator tests
```

## Test Framework

Tests use [NUnit](https://nunit.org/) with `Microsoft.NET.Test.Sdk` and `NUnit3TestAdapter`.

## .NET Requirement

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Running Tests

```bash
# Run all tests
dotnet test src/EvaluationSLN/EvaluationSLN.slnx

# Run tests for a specific project
dotnet test src/EvaluationSLN/Eval.BuiltIn.Coherence/Eval.BuiltIn.Coherence.csproj

# Run tests with detailed output
dotnet test src/EvaluationSLN/EvaluationSLN.slnx --logger "console;verbosity=detailed"
```

## Writing Tests

Tests for AI evaluation scenarios typically fall into two categories:

1. **Unit tests** — Test evaluator logic with fixed inputs and expected outputs (no LLM calls)
2. **Integration tests** — Run end-to-end evaluation scenarios against live Azure resources

> ⚠️ Integration tests require valid Azure credentials and resource configurations in `.env`.

### Example Unit Test

```csharp
using Microsoft.Extensions.AI.Evaluation;
using Xunit;

public class EvaluationTests
{
    [Fact]
    public async Task Evaluator_ReturnsMetric_ForValidInput()
    {
        // Arrange
        var evaluator = new CoherenceEvaluator();
        var messages = new[] { new ChatMessage(ChatRole.User, "What is .NET?") };
        var response = ".NET is a free, open-source developer platform.";

        // Act
        EvaluationResult result = await evaluator.EvaluateAsync(messages, response);

        // Assert
        Assert.True(result.Metrics.ContainsKey(CoherenceEvaluator.CoherenceMetricName));
    }
}
```

## References

- [NUnit documentation](https://docs.nunit.org/)
- [AI Evaluation API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
