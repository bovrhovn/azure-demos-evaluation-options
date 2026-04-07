# Tests

This folder contains unit and integration tests for the evaluation demos in this repository.

## Structure

```
tests/
├── agent-framework/    # Tests for the Agent Framework evaluation demos
└── foundry-sdk/        # Tests for the Azure AI Foundry SDK evaluation demos
```

## Test Framework

Tests use [xUnit](https://xunit.net/) as the test framework, consistent with the broader .NET ecosystem and the [official AI evaluation samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api).

## Running Tests

```bash
# Run all tests
dotnet test tests/

# Run tests for a specific project
dotnet test tests/agent-framework/

# Run tests with detailed output
dotnet test tests/ --logger "console;verbosity=detailed"
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

- [xUnit documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [AI Evaluation API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
