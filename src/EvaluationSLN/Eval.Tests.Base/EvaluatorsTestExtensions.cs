using Azure.AI.Projects;
using Azure.AI.Projects.Evaluation;

namespace Eval.Tests.Base;

public class EvaluatorsTestExtensions
{
    public static EvaluatorVersion GetPromptEvaluatorVersion()
    {
        EvaluatorMetric metric = new()
        {
            Type = EvaluatorMetricType.Ordinal,
            DesirableDirection = EvaluatorMetricDirection.Increase,
            MinValue = 1,
            MaxValue = 5
        };
        return new(
            categories: [EvaluatorCategory.Quality],
            definition: new PromptBasedEvaluatorDefinition(
                promptText: """
                    You are an evaluator.
                    Rate the GROUNDEDNESS (factual correctness without unsupported claims) of the system response to the customer query.

                    Scoring (1–5):
                    1 = Mostly fabricated/incorrect
                    2 = Many unsupported claims
                    3 = Mixed: some facts but notable errors/guesses
                    4 = Mostly factual; minor issues
                    5 = Fully factual; no unsupported claims

                    Return ONLY a single integer 1–5 as score in valid json response e.g {\"score\": int}.

                    Query:
                    {query}

                    Response:
                    {response}
                    """,
                initParameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            deployment_name = new { type = "string" },
                            threshold = new { rtpe = "number" },
                        },
                        required = new[] { "deployment_name", "threshold" }
                    }
                ),
                dataSchema: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            query = new { type = "string" },
                            response = new { type = "string" }
                        }
                    }
                ),
                metrics: new Dictionary<string, EvaluatorMetric> { { "score", metric } }
            ),
            evaluatorType: EvaluatorType.Custom
        )
        {
            DisplayName = "my_custom_evaluator",
            Description = "Custom evaluator to detect violent content",
        };
    }
    
    public static EvaluatorVersion GetCodeEvaluatorVersion()
    {
        EvaluatorMetric resultMetric = new()
        {
            Type = EvaluatorMetricType.Ordinal,
            DesirableDirection = EvaluatorMetricDirection.Increase,
            MinValue = 0,
            MaxValue = 5
        };
        EvaluatorVersion evaluatorVersion = new(
            categories: [EvaluatorCategory.Quality],
            definition: new CodeBasedEvaluatorDefinition(
                codeText: "def grade(sample, item):\n    return 1.0",
                initParameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            deployment_name = new { type = "string" },
                        },
                        required = new[] { "deployment_name" },
                    }
                ),
                dataSchema: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            item = new { type = "string" },
                            response = new { type = "string" }
                        },
                        required = new[] { "query", "response" },
                    }
                ),
                metrics: new Dictionary<string, EvaluatorMetric> {
                    { "result", resultMetric }
                }
            ),
            evaluatorType: EvaluatorType.Custom
        )
        {
            DisplayName = "my_custom_evaluator",
            Description = "Custom evaluator to detect violent content",
        };
        return evaluatorVersion;
    }

    public static void DisplayEvaluatorVersion(EvaluatorVersion evaluator)
    {
        Console.WriteLine($"Evaluator ID: {evaluator.Id}");
        Console.WriteLine($"    Name: {evaluator.Name}");
        Console.WriteLine($"    Version: {evaluator.Version}");
        Console.WriteLine("     Categories:");
        foreach (EvaluatorCategory category in evaluator.Categories)
        {
            Console.WriteLine("         - ${category}");
        }
    }
}