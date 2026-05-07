using System.ClientModel;
using System.Text;
using System.Text.Json;
using OpenAI.Evals;

namespace Eval.Tests.Base;

public class EvalTestExtensions
{
    public static string GetErrorMessageOrEmpty(ClientResult result)
    {
        string error = string.Empty;
        Utf8JsonReader reader = new(result.GetRawResponse().Content.ToMemory().ToArray());
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        string code = string.Empty;
        string message = string.Empty;
        foreach (JsonProperty prop in document.RootElement.EnumerateObject())
        {
            if (prop.NameEquals("error"u8) && prop.Value.ValueKind != JsonValueKind.Null && prop.Value is JsonElement countsElement)
            {
                foreach (JsonProperty errorNode in countsElement.EnumerateObject())
                {
                    if (errorNode.Value.ValueKind == JsonValueKind.String)
                    {
                        if (errorNode.NameEquals("code"u8))
                        {
                            code = errorNode.Value.GetString() ?? "";
                        }
                        else if (errorNode.NameEquals("message"u8))
                        {
                            message = errorNode.Value.GetString() ?? "";
                        }
                    }
                }
            }
        }
        if (!string.IsNullOrEmpty(message))
        {
            error = $"Message: {message}, Code: {code ?? "<None>"}";
        }
        return error;
    }

    public static async Task<List<string>> GetResultsListAsync(EvaluationClient client, string evaluationId, string evaluationRunId)
    {
        List<string> resultJsons = [];
        var hasMore = false;
        var after = string.Empty;
        do
        {
            var resultList = await client.GetEvaluationRunOutputItemsAsync(evaluationId: evaluationId, evaluationRunId: evaluationRunId, limit: null, order: "asc", after: after, outputItemStatus: default, options: new());
            Utf8JsonReader reader = new(resultList.GetRawResponse().Content.ToMemory().ToArray());
            using var document = JsonDocument.ParseValue(ref reader);

            foreach (var topProperty in document.RootElement.EnumerateObject())
            {
                if (topProperty.NameEquals("has_more"u8))
                {
                    hasMore = topProperty.Value.GetBoolean();
                }
                else if (topProperty.NameEquals("data"u8))
                {
                    if (topProperty.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var dataElement in topProperty.Value.EnumerateArray())
                        {
                            resultJsons.Add(dataElement.ToString());
                        }
                    }
                }
                else if (topProperty.NameEquals("last_id"u8))
                {
                    after = topProperty.Value.GetString() ?? "";
                }
            }
        } while (hasMore);
        return resultJsons;
    }

    public static string GetResultsCounts(ClientResult result)
    {
        Utf8JsonReader reader = new(result.GetRawResponse().Content.ToMemory().ToArray());
        using var document = JsonDocument.ParseValue(ref reader);
        StringBuilder sbFormattedCounts = new("{\n");
        foreach (var prop in document.RootElement.EnumerateObject())
        {
            if (prop.NameEquals("result_counts"u8) && prop.Value is var countsElement)
            {
                foreach (var count in countsElement.EnumerateObject())
                {
                    if (count.Value.ValueKind == JsonValueKind.Number)
                    {
                        sbFormattedCounts.Append($"    {count.Name}: {count.Value.GetInt32()}\n");
                    }
                }
            }
        }
        sbFormattedCounts.Append('}');
        if (sbFormattedCounts.Length == 3)
        {
            throw new InvalidOperationException("The result does not contain the \"result_counts\" field.");
        }
        return sbFormattedCounts.ToString();
    }

    public static Dictionary<string, string> ParseClientResult(ClientResult result, string[] expectedProperties)
    {
        Dictionary<string, string> results = [];
        Utf8JsonReader reader = new(result.GetRawResponse().Content.ToMemory().ToArray());
        using var document = JsonDocument.ParseValue(ref reader);
        foreach (var prop in document.RootElement.EnumerateObject())
        {
            foreach (var key in expectedProperties)
            {
                if (prop.NameEquals(Encoding.UTF8.GetBytes(key)) && prop.Value.ValueKind == JsonValueKind.String)
                {
                    results[key] = prop.Value.GetString() ?? "";
                }
            }
        }

        List<string> notFoundItems = [.. expectedProperties.Where((key) => !results.ContainsKey(key))];
        if (notFoundItems.Count > 0)
        {
            StringBuilder sbNotFound = new();
            foreach (var value in notFoundItems)
            {
                sbNotFound.Append($"{value}, ");
            }

            if (sbNotFound.Length > 2)
            {
                sbNotFound.Remove(sbNotFound.Length - 2, 2);
            }

            throw new InvalidOperationException($"The next keys were not found in returned result: {sbNotFound}.");
        }

        return results;
    }

    public static BinaryData GetData()
    {
        object dataSource = new
        {
            type = "jsonl",
            source = new
            {
                type = "file_content",
                content = new[]
                {
                    new
                    {
                        item = new
                        {
                            query = "What are some tips for staying healthy?",
                            context = "Health and wellness advice",
                            ground_truth = "Exercise regularly, eat balanced meals, and get enough sleep",
                            response =
                                "To stay healthy, focus on regular exercise, a balanced diet, adequate sleep, and stress management."
                        }
                    },
                    new
                    {
                        item = new
                        {
                            query = "How do I improve my writing skills?",
                            context = "Writing improvement techniques",
                            ground_truth = "Practice regularly and read widely",
                            response = "Read extensively, write daily, seek feedback, and study grammar fundamentals."
                        }
                    },
                    new
                    {
                        item = new
                        {
                            query = "What is the capital of France?",
                            context = "Geography question about European capitals",
                            ground_truth = "Paris",
                            response = "The capital of France is Paris."
                        }
                    },
                    new
                    {
                        item = new
                        {
                            query = "Explain quantum computing",
                            context = "Complex scientific concept explanation",
                            ground_truth = "Quantum computing uses quantum mechanics principles",
                            response =
                                "Quantum computing leverages quantum mechanical phenomena like superposition and entanglement to process information."
                        }
                    },
                }
            }
        };

        return BinaryData.FromObjectAsJson(
            new
            {
                name = "inline_data_run",
                metadata = new { team = "eval-exp", scenario = "inline-data-v1" },
                data_source = dataSource
            }
        );
    }

    public static BinaryData GetDataConfig(string modelDeploymentName)
    {
        object dataSourceConfig = new
        {
            type = "custom",
            item_schema = new
            {
                type = "object",
                properties = new
                {
                    query = new { type = "string" },
                    response = new { type = "string" },
                    context = new { type = "string" },
                    ground_truth = new { type = "string" }
                },
                required = Array.Empty<string>()
            },
            include_sample_schema = true
        };

        object[] testingCriteria =
        [
            new
            {
                type = "azure_ai_evaluator",
                name = "coherence",
                evaluator_name = "builtin.coherence",
                initialization_parameters = new { deployment_name = modelDeploymentName }
            },
        ];

        return BinaryData.FromObjectAsJson(
            new
            {
                name = "label model test with inline data",
                data_source_config = dataSourceConfig,
                testing_criteria = testingCriteria
            }
        );
    }
}