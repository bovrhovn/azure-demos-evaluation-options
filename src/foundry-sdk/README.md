# Azure AI Foundry SDK Evaluation Demos

This folder contains demos that evaluate AI models and prompts using the [Azure AI Foundry SDK](https://learn.microsoft.com/en-us/azure/ai-foundry/) with the `Microsoft.Extensions.AI.Evaluation` libraries.

## What is Azure AI Foundry?

[Azure AI Foundry](https://ai.azure.com) is Microsoft's unified platform for building, evaluating, and deploying AI applications. It provides:

- A **model catalog** with hundreds of foundation models
- Built-in **evaluation** capabilities for quality and safety
- **Azure AI Projects** client SDK for programmatic access
- Integration with `Microsoft.Extensions.AI` and `Azure.AI.Inference`

## Evaluation Approach

Azure AI Foundry evaluation demos in this folder:

1. **Connect to an Azure AI Foundry project** via the `Azure.AI.Projects` SDK
2. **Send chat requests** through an `IChatClient` backed by Azure AI
3. **Apply evaluators** from `Microsoft.Extensions.AI.Evaluation` to score responses
4. **Persist results** using `ChatConfiguration` with a `DiskBasedResponseCache`

### Evaluators Used

| Evaluator | Package | What it measures |
|---|---|---|
| `RelevanceTruthAndCompletenessEvaluator` | `Quality` | Relevance, truth, and completeness of responses |
| `FluencyEvaluator` | `Quality` | Writing quality and fluency |
| `GroundednessEvaluator` | `Quality` | Grounding in provided context |

## Projects in this Folder

> Projects will be added as the demos are developed. See [official API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api) for reference patterns.

## References

- [Azure AI Foundry documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/)
- [Azure AI Foundry SDK for .NET](https://learn.microsoft.com/en-us/azure/ai-foundry/how-to/develop/sdk-overview)
- [Microsoft.Extensions.AI.Evaluation libraries](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- [AI Evaluation API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
