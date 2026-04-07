# Evaluation with Microsoft Agent Framework

## Overview

The [Semantic Kernel Agent Framework](https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/) (part of the Microsoft Agent Framework) enables building AI agents that use tools, maintain state, and collaborate in multi-agent scenarios. This guide explains how to evaluate those agents using `Microsoft.Extensions.AI.Evaluation`.

## Required NuGet Packages

```xml
<PackageReference Include="Microsoft.Extensions.AI.Evaluation" Version="*" />
<PackageReference Include="Microsoft.Extensions.AI.Evaluation.Quality" Version="*" />
<PackageReference Include="Microsoft.Extensions.AI.Evaluation.Reporting" Version="*" />
<PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="*" />
<PackageReference Include="Microsoft.SemanticKernel.Connectors.AzureOpenAI" Version="*" />
```

## Setting Up Evaluation

Use `ChatConfiguration` to configure how evaluation results are cached and reported:

```csharp
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

// Storage path for evaluation results (e.g., disk-based cache)
string storagePath = Path.Combine(Path.GetTempPath(), "evaluation-results");

ChatConfiguration chatConfig = new(
    endpoint: new AzureOpenAIChatConfiguration(
        deploymentName: "gpt-4o",
        endpoint: new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!),
        apiKey: Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")!
    ),
    resultStorage: new DiskBasedResponseCache(storagePath)
);
```

## Running an Evaluation

```csharp
using Microsoft.Extensions.AI.Evaluation.Quality;

// Define evaluators
IEvaluator[] evaluators =
[
    new RelevanceTruthAndCompletenessEvaluator(),
    new CoherenceEvaluator(),
];

// Run evaluation
EvaluationResult result = await evaluators.EvaluateAsync(
    messages: conversationHistory,
    modelResponse: agentResponse,
    chatConfig: chatConfig
);

// Review scores
foreach (EvaluationMetric metric in result.Metrics.Values)
{
    Console.WriteLine($"{metric.Name}: {metric.Value} — {metric.Interpretation}");
}
```

## Integrating with Semantic Kernel Agents

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

Kernel kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

ChatCompletionAgent agent = new()
{
    Name = "EvaluationAgent",
    Instructions = "You are a helpful assistant.",
    Kernel = kernel,
};

// Collect responses and evaluate
AgentThread thread = new();
await foreach (ChatMessageContent message in agent.InvokeAsync("Tell me about .NET 9.", thread))
{
    // Evaluate each response
    EvaluationResult result = await evaluators.EvaluateAsync(
        messages: thread.History,
        modelResponse: message.Content!,
        chatConfig: chatConfig
    );
}
```

## References

- [Semantic Kernel Agent Framework docs](https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/)
- [Microsoft.Extensions.AI.Evaluation libraries](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- [Official samples — AI Evaluation API](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
