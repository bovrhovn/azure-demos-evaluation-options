# Evaluation with Microsoft Foundry SDK

## Overview

[Azure AI Foundry](https://ai.azure.com) is Microsoft's unified AI platform for building, evaluating, and deploying AI applications. In this solution, Foundry integration uses the Microsoft Foundry SDK (`Azure.AI.Projects`). This guide explains how to evaluate AI models using `Microsoft.Extensions.AI.Evaluation`.

## Required NuGet Packages

```xml
<PackageReference Include="Microsoft.Extensions.AI.Evaluation" Version="*" />
<PackageReference Include="Microsoft.Extensions.AI.Evaluation.Quality" Version="*" />
<PackageReference Include="Microsoft.Extensions.AI.Evaluation.Reporting" Version="*" />
<PackageReference Include="Azure.AI.Projects" Version="*" />
<PackageReference Include="Azure.AI.Inference" Version="*" />
<PackageReference Include="Microsoft.Extensions.AI.AzureAIInference" Version="*" />
```

## Setting Up the Microsoft Foundry SDK Client

```csharp
using Azure.AI.Projects;
using Azure.Identity;

string connectionString = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_CONNECTION_STRING")!;

AIProjectClient projectClient = new(connectionString, new DefaultAzureCredential());
ChatCompletionsClient chatClient = projectClient.GetChatCompletionsClient();
```

## Setting Up Evaluation

Use `ChatConfiguration` to wire the Microsoft Foundry SDK client into the evaluation pipeline:

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

IChatClient client = chatClient.AsIChatClient(modelId: "gpt-4o");

string storagePath = Path.Combine(Path.GetTempPath(), "evaluation-results");

ChatConfiguration chatConfig = new(
    chatClient: client,
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
    new FluencyEvaluator(),
    new GroundednessEvaluator(),
];

// Send a chat request
IList<ChatMessage> messages =
[
    new ChatMessage(ChatRole.System, "You are a helpful assistant."),
    new ChatMessage(ChatRole.User, "What are the new features in .NET 9?"),
];

ChatResponse response = await client.GetResponseAsync(messages);

// Evaluate the response
EvaluationResult result = await evaluators.EvaluateAsync(
    messages: messages,
    modelResponse: response.Text,
    chatConfig: chatConfig
);

foreach (EvaluationMetric metric in result.Metrics.Values)
{
    Console.WriteLine($"{metric.Name}: {metric.Value} — {metric.Interpretation}");
}
```

## Generating Reports

The `DiskBasedResponseCache` stores results to disk, which can be converted into HTML reports:

```csharp
using Microsoft.Extensions.AI.Evaluation.Reporting;

// Read stored results and generate a report
EvaluationReport report = await EvaluationReport.CreateAsync(storagePath);
await report.SaveAsHtmlAsync("evaluation-report.html");
```

## References

- [Azure AI Foundry documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/)
- [Microsoft Foundry SDK for .NET](https://learn.microsoft.com/en-us/azure/ai-foundry/how-to/develop/sdk-overview)
- [Microsoft.Extensions.AI.Evaluation libraries](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- [Official samples — AI Evaluation API](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
