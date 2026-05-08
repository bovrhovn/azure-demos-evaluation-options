using System.ClientModel;
using Azure.AI.Projects;
using Azure.Identity;
using Eval.Tests.Base;
using OpenAI.Evals;

namespace Eval.Agent.AIAssistedEvaluation;

public class TestAIAssistedEvaluation
{
    private AIProjectClient projectClient;
    private EvaluationClient evaluationClient;

    [SetUp]
    public void Setup()
    {
        projectClient = new(new Uri(Environment.GetEnvironmentVariable("ENDPOINT") ??
                                    "https://ai-common.services.ai.azure.com/api/projects/agentic-ai-project"),
            new DefaultAzureCredential());
        evaluationClient = projectClient.ProjectOpenAIClient.GetEvaluationClient();
    }
    
    [Test]
    public async Task TestAssistedEvaluation()
    {
        var modelDeploymentName = Environment.GetEnvironmentVariable("DEPLOYMENT_NAME") ?? 
                                  "general-gpt-4.1";
        using var evaluationDataContent = BinaryContent.Create(GetDataConfig(modelDeploymentName));
        var evaluation = await evaluationClient.CreateEvaluationAsync(evaluationDataContent);
        var fields = EvalTestExtensions.ParseClientResult(evaluation, ["name", "id"]);
        var evaluationName = fields["name"];
        var evaluationId = fields["id"];
        Console.WriteLine($"Evaluation created (id: {evaluationId}, name: {evaluationName})");
        var evaluationResponse = await evaluationClient.GetEvaluationAsync(evaluationId, new());
        Console.WriteLine($"Retrieved evaluation: {evaluationResponse.GetRawResponse().Content}");
        using var runDataContent = BinaryContent.Create(GetData());
        var run = await evaluationClient.CreateEvaluationRunAsync(evaluationId: evaluationId, content: runDataContent);
        fields = EvalTestExtensions.ParseClientResult(run, ["id", "status"]);
        var runId = fields["id"];
        var runStatus = fields["status"];
        Console.WriteLine($"Evaluation run created (id: {runId})");
        while (runStatus != "failed" && runStatus != "completed")
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            run = await evaluationClient.GetEvaluationRunAsync(evaluationId: evaluationId, evaluationRunId: runId, options: new());
            runStatus = EvalTestExtensions.ParseClientResult(run, ["status"])["status"];
            Console.WriteLine($"Waiting for eval run to complete... current status: {runStatus}");
        }
        if (runStatus == "failed")
        {
            throw new InvalidOperationException($"Evaluation run failed with error: {EvalTestExtensions.GetErrorMessageOrEmpty(run)}");
        }
        Console.WriteLine("Evaluation run completed successfully!");
        Console.WriteLine($"Result Counts: {EvalTestExtensions.GetResultsCounts(run)}");
        var evaluationResults = await EvalTestExtensions.GetResultsListAsync(client: evaluationClient, evaluationId: evaluationId, evaluationRunId: runId);
        Console.WriteLine($"OUTPUT ITEMS (Total: {evaluationResults.Count})");
        Console.WriteLine($"------------------------------------------------------------");
        foreach (var result in evaluationResults)
        {
            Console.WriteLine(result);
        }
        Console.WriteLine($"------------------------------------------------------------");
        Assert.Pass("Test for AI Assisted Evaluation. This test is to verify that the evaluation can be created successfully. The actual evaluation will be run in the pipeline and the results will be verified there.");
        //cleanup
        // await evaluationClient.DeleteEvaluationAsync(evaluationId, new System.ClientModel.Primitives.RequestOptions());
        // Console.WriteLine("Evaluation deleted");
    }
   
    private static BinaryData GetData()
    {
        object dataSource = new
        {
            type = "jsonl",
            source = new
            {
                type = "file_content",
                content = new[]
                {
                    new {
                        item = new {
                            response = "The capital of France is Paris, which is also known as the City of Light.",
                            ground_truth = "Paris is the capital of France."
                        }
                    },
                    new {
                        item = new {
                            response = "Python is a high-level programming language known for its simplicity and readability.",
                            ground_truth = "Python is a popular programming language that is easy to learn."
                        }
                    },
                    new {
                        item = new {
                            response = "Machine learning is a subset of artificial intelligence that enables systems to learn from data.",
                            ground_truth = "Machine learning allows computers to learn from data without being explicitly programmed."
                        }
                    },
                    new {
                        item = new {
                            response = "The sun rises in the east and sets in the west due to Earth's rotation.",
                            ground_truth = "The sun appears to rise in the east and set in the west because of Earth's rotation."
                        }
                    },
                }
            }
        };
        return BinaryData.FromObjectAsJson(
            new
            {
                name = "inline_data_ai_assisted_run",
                metadata = new { team = "eval-exp", scenario = "ai-assisted-inline-v1" },
                data_source = dataSource
            }
        );
    }
    
    private static BinaryData GetDataConfig(string modelDeploymentName)
    {
        object dataSourceConfig = new
        {
            type = "custom",
            item_schema = new
            {
                type = "object",
                properties = new
                {
                    response = new { type = "string" },
                    ground_truth = new { type = "string" }
                },
                required = Array.Empty<string>()
            },
            include_sample_schema = false
        };

        object[] testingCriteria = [
            // new {
            //     type = "azure_ai_evaluator",
            //     name = "Similarity",
            //     evaluator_name = "builtin.similarity",
            //     data_mapping = new { response = "{{item.response}}", ground_truth = "{{item.ground_truth}}" },
            //     initialization_parameters = new { deployment_name = modelDeploymentName, threshold = 3 }
            // },
            new {
                type = "azure_ai_evaluator",
                name = "ROUGEScore",
                evaluator_name = "builtin.rouge_score",
                data_mapping = new { response = "{{item.response}}", ground_truth = "{{item.ground_truth}}" },
                initialization_parameters = new { rouge_type = "rouge1", f1_score_threshold = 0.5, precision_threshold = 0.5, recall_threshold = 0.5 }
            },
            new {
                type = "azure_ai_evaluator",
                name = "METEORScore",
                evaluator_name = "builtin.meteor_score",
                data_mapping = new { response = "{{item.response}}", ground_truth = "{{item.ground_truth}}" },
                initialization_parameters = new { threshold = 0.5 }
            },
            new {
                type = "azure_ai_evaluator",
                name = "GLEUScore",
                evaluator_name = "builtin.gleu_score",
                data_mapping = new { response = "{{item.response}}", ground_truth = "{{item.ground_truth}}" },
                initialization_parameters = new { threshold = 0.5 }
            },
            new {
                type = "azure_ai_evaluator",
                name = "F1Score",
                evaluator_name = "builtin.f1_score",
                data_mapping = new { response = "{{item.response}}", ground_truth = "{{item.ground_truth}}" },
                initialization_parameters = new { threshold = 0.5 }
            },
            new {
                type = "azure_ai_evaluator",
                name = "BLEUScore",
                evaluator_name = "builtin.bleu_score",
                data_mapping = new { response = "{{item.response}}", ground_truth = "{{item.ground_truth}}" },
                initialization_parameters = new { threshold = 0.5 }
            },
        ];

        return BinaryData.FromObjectAsJson(
            new
            {
                name = "AI assisted evaluators test",
                data_source_config = dataSourceConfig,
                testing_criteria = testingCriteria
            }
        );
    }
}