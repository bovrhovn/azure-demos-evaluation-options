# AI Evaluation Concepts

## What is AI Evaluation?

AI evaluation is the process of systematically measuring the quality, safety, and effectiveness of AI-generated outputs. Rather than manually reviewing every AI response, evaluation frameworks automate scoring using a set of well-defined metrics.

## Why Evaluate AI Applications?

- **Catch regressions** — Ensure that model or prompt changes don't degrade response quality
- **Measure safety** — Detect harmful, biased, or inappropriate outputs before they reach users
- **Compare approaches** — Objectively compare different models, prompts, or agent configurations
- **Build confidence** — Provide evidence that your AI application meets quality standards

## Evaluation Metrics

### Quality Metrics

| Metric | What it measures |
|---|---|
| **Relevance** | Does the response address the user's question? |
| **Truth** | Is the response factually accurate given the context? |
| **Completeness** | Does the response fully answer the question? |
| **Coherence** | Is the response logically consistent and well-structured? |
| **Fluency** | Is the response well-written in natural language? |
| **Groundedness** | Is the response supported by the provided context (RAG scenarios)? |

### Safety Metrics

| Metric | What it measures |
|---|---|
| **Hateful content** | Does the response contain hate speech? |
| **Violent content** | Does the response contain violent content? |
| **Sexual content** | Does the response contain explicit content? |
| **Self-harm content** | Does the response promote self-harm? |

## Evaluator Types in Microsoft.Extensions.AI.Evaluation

### LLM-based Evaluators

Some evaluators (like `RelevanceTruthAndCompletenessEvaluator`) use a second LLM call to score responses. They:
- Send the original context + response to a "judge" LLM
- Parse structured output to extract a numeric score and reasoning
- Require an `IChatClient` configured via `ChatConfiguration`

### Rule-based Evaluators

Rule-based evaluators use deterministic logic (e.g., checking for specific patterns, word counts, etc.) and do not require an LLM.

## Evaluation Pipeline

```
Input Messages
      │
      ▼
 AI Application  ──────►  Response
      │                       │
      └───────────────────────┘
                  │
                  ▼
            Evaluators
         (Quality + Safety)
                  │
                  ▼
          EvaluationResult
       (metrics + diagnostics)
                  │
                  ▼
         Storage / Report
    (DiskBasedResponseCache)
```

## Best Practices

1. **Evaluate early and often** — Run evaluations as part of your CI/CD pipeline
2. **Use diverse test sets** — Cover edge cases, adversarial inputs, and normal use cases
3. **Version your evaluations** — Track metrics over time to detect regressions
4. **Combine metrics** — No single metric captures everything; use a suite of evaluators
5. **Review failures** — Low scores are opportunities to improve prompts or fine-tuning

## References

- [Microsoft.Extensions.AI.Evaluation libraries](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- [AI Evaluation API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)
- [Responsible AI overview](https://learn.microsoft.com/en-us/azure/ai-foundry/responsible-use-of-ai-overview)
