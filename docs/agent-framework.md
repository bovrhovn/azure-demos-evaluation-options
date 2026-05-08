# Evaluation with Microsoft Agent Framework

## Overview

The Microsoft Agent Framework enables building AI agents that can use tools, maintain state, and participate in orchestrated workflows. This guide explains how to evaluate those agents using `Microsoft.Extensions.AI.Evaluation` with Microsoft Foundry SDK connectivity.

## Required NuGet Packages

```xml
<PackageReference Include="Microsoft.Extensions.AI.Evaluation" Version="*" />
<PackageReference Include="Microsoft.Extensions.AI.Evaluation.Quality" Version="*" />
<PackageReference Include="Microsoft.Extensions.AI.Evaluation.Reporting" Version="*" />
<PackageReference Include="Microsoft.Agents.AI.Foundry" Version="*" />
<PackageReference Include="Azure.AI.Projects" Version="*" />
<PackageReference Include="Azure.Identity" Version="*" />
```

## Setting Up Evaluation

Use `ChatConfiguration` to configure how evaluation results are cached and reported:

```csharp
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

string endpoint = Environment.GetEnvironmentVariable("ENDPOINT")!;
string deploymentName = Environment.GetEnvironmentVariable("DEPLOYMENT_NAME")!;

AIAgent agent = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential())
    .AsAIAgent(
        model: deploymentName,
        instructions: "You are a helpful assistant.",
        name: "EvaluationAgent");

string storagePath = Path.Combine(Path.GetTempPath(), "evaluation-results");

ChatConfiguration chatConfig = new(
    chatClient: agent.AsIChatClient(),
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

// Run the agent and evaluate its response
string agentResponse = (await agent.RunAsync("Tell me about .NET 9.")).Text;

EvaluationResult result = await evaluators.EvaluateAsync(
    messages: [new ChatMessage(ChatRole.User, "Tell me about .NET 9.")],
    modelResponse: agentResponse,
    chatConfig: chatConfig
);

// Review scores
foreach (EvaluationMetric metric in result.Metrics.Values)
{
    Console.WriteLine($"{metric.Name}: {metric.Value} — {metric.Interpretation}");
}
```

## Integrating with Microsoft Agent Framework

```csharp
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;

AIProjectClient projectClient = new(new Uri(endpoint), new DefaultAzureCredential());

AIAgent agent = projectClient.AsAIAgent(
    model: deploymentName,
    instructions: "You are a helpful assistant.",
    name: "EvaluationAgent");

// Collect responses and evaluate
string response = (await agent.RunAsync("Tell me about .NET 9.")).Text;
EvaluationResult result = await evaluators.EvaluateAsync(
    messages: [new ChatMessage(ChatRole.User, "Tell me about .NET 9.")],
    modelResponse: response,
    chatConfig: chatConfig
);
```

## References

- [Azure AI Foundry Agents documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/agents/)
- [Microsoft Foundry SDK for .NET](https://learn.microsoft.com/en-us/azure/ai-foundry/how-to/develop/sdk-overview)
- [Microsoft.Extensions.AI.Evaluation libraries](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- [Official samples — AI Evaluation API](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
