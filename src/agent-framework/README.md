# Agent Framework Evaluation Demos

This folder contains demos that evaluate AI agents built with the Microsoft Agent Framework using the `Microsoft.Extensions.AI.Evaluation` libraries.

## What is the Microsoft Agent Framework?

The Microsoft Agent Framework provides abstractions for building AI agents that can:

- Use tools and plugins
- Maintain conversation history
- Collaborate in multi-agent scenarios (e.g., `AgentGroupChat`)
- Integrate with Azure OpenAI and other AI backends

## Evaluation Approach

Agents are evaluated by:

1. **Running a conversation** — the agent responds to a set of predefined prompts
2. **Applying evaluators** — each response is scored using `Microsoft.Extensions.AI.Evaluation` evaluators
3. **Generating a report** — results are stored via `ChatConfiguration` and surfaced as a structured report

### Evaluators Used

| Evaluator | Package | What it measures |
|---|---|---|
| `RelevanceTruthAndCompletenessEvaluator` | `Quality` | Relevance, truth, and completeness of responses |
| `CoherenceEvaluator` | `Quality` | Logical consistency of the response |
| `GroundednessEvaluator` | `Quality` | Whether responses are grounded in provided context |

## Projects in this Folder

> Projects will be added as the demos are developed. See [official API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api) for reference patterns.

## References

- [Azure AI Foundry Agents documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/agents/)
- [Microsoft.Extensions.AI.Evaluation libraries](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- [AI Evaluation API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
