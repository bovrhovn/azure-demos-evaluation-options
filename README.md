# Azure AI Evaluation Options

> Demonstrations of AI evaluation tools using the official .NET SDK — covering Microsoft Agent Framework, Microsoft Foundry SDK, and built-in evaluator scenarios.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Overview

Modern AI applications require robust evaluation strategies to ensure quality, safety, and reliability of AI-generated outputs. This repository showcases how to use the official [Microsoft.Extensions.AI.Evaluation](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries) libraries to evaluate AI models and agents in a structured, repeatable way.

Three complementary approaches are demonstrated:

| Approach | Description |
|---|---|
| **Microsoft Agent Framework** | Evaluate AI agents built with the Microsoft Agent Framework, including orchestration and reusable agent patterns |
| **Microsoft Foundry SDK** | Evaluate models and prompts using [Azure AI Foundry](https://learn.microsoft.com/en-us/azure/ai-foundry/) through the `Azure.AI.Projects` SDK |
| **Built-in Evaluators** | Run built-in quality and safety evaluator scenarios such as coherence, violence detection, BLEU score, and evaluator catalog exploration |

Both approaches build on the same `Microsoft.Extensions.AI.Evaluation` NuGet packages, enabling a consistent evaluation workflow regardless of which AI stack you use.

---

## Project Structure

```
azure-demos-evaluation-options/
├── src/
│   └── EvaluationSLN/                   # .NET 10 solution and demo projects
│       ├── Eval.Agent/                  # Agent Framework scenario
│       ├── Eval.FoundryAgent/           # Foundry agent scenario
│       ├── Eval.Agent.AIAssistedEvaluation/  # AI-assisted agent evaluation tests
│       ├── Eval.BuiltIn.AgentEvaluation/
│       ├── Eval.BuiltIn.BleuScoreTests/
│       ├── Eval.BuiltIn.Coherence/
│       ├── Eval.BuiltIn.EvaluatorsCatalog/
│       ├── Eval.BuiltIn.ViolenceDetection/
│       └── Eval.Tests.Base/
├── docs/                                # Documentation and guides
├── scripts/                             # Setup and utility scripts
└── tests/                               # Test documentation
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Azure subscription](https://azure.microsoft.com/free/)
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
- An Azure OpenAI or Azure AI Foundry resource

### Quick Start

1. **Clone the repository**

   ```bash
   git clone https://github.com/bovrhovn/azure-demos-evaluation-options.git
   cd azure-demos-evaluation-options
   ```

2. **Set up your environment**

   ```bash
   # Copy the environment template and fill in your values
   cp scripts/.env.template .env
   ```

3. **Run setup script**

   ```bash
   bash scripts/setup.sh
   ```

4. **Explore the demos**
   - [`src/README.md`](src/README.md) — Source layout and project overview
   - [`src/EvaluationSLN/EvaluationSLN.slnx`](src/EvaluationSLN/EvaluationSLN.slnx) — Solution containing all demo projects

---

## Key Concepts

### What is AI Evaluation?

AI evaluation is the process of systematically measuring the quality, safety, and effectiveness of AI-generated outputs. This includes:

- **Relevance** — Is the response relevant to the input?
- **Groundedness** — Is the response grounded in the provided context?
- **Coherence** — Is the response logically consistent?
- **Fluency** — Is the response well-written?
- **Safety** — Does the response avoid harmful content?

### Microsoft.Extensions.AI.Evaluation

The [`Microsoft.Extensions.AI.Evaluation`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai.evaluation) namespace provides evaluators and scaffolding for running AI evaluations in .NET applications. Key packages:

| NuGet Package | Purpose |
|---|---|
| `Microsoft.Extensions.AI.Evaluation` | Core evaluation abstractions |
| `Microsoft.Extensions.AI.Evaluation.Quality` | Quality evaluators (relevance, coherence, groundedness, etc.) |
| `Microsoft.Extensions.AI.Evaluation.Safety` | Safety evaluators (harmful content detection) |
| `Microsoft.Extensions.AI.Evaluation.Reporting` | Evaluation result storage and reporting |

---

## Official Resources

### Documentation

- 📖 [AI Evaluation Libraries (.NET)](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- 📖 [Microsoft.Extensions.AI overview](https://learn.microsoft.com/en-us/dotnet/ai/microsoft-extensions-ai)
- 📖 [Azure AI Foundry Agents documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/agents/)
- 📖 [Azure AI Foundry documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/)

### Official Samples

- 🔗 [dotnet/ai-samples — Microsoft.Extensions.AI.Evaluation API samples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation/api)

---

## Repository Documentation

| Document | Description |
|---|---|
| [docs/overview.md](docs/overview.md) | Detailed project overview and architecture |
| [docs/agent-framework.md](docs/agent-framework.md) | Guide for the Agent Framework evaluation demos |
| [docs/foundry-sdk.md](docs/foundry-sdk.md) | Guide for the Microsoft Foundry SDK evaluation demos |
| [docs/evaluation-concepts.md](docs/evaluation-concepts.md) | Explanation of AI evaluation concepts used in this repo |

---

## Contributing

Contributions are welcome! Please read the [contribution guidelines](docs/CONTRIBUTING.md) before submitting a pull request.

---

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
