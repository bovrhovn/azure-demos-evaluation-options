using System.ClientModel;
using Azure.AI.Projects;
using Azure.Identity;
using Eval.Tests.Base;
using OpenAI.Evals;

namespace Eval.BuiltIn.Coherence;

public class TestsCoherenceInline
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
    //[AsyncOnly]
    public async Task BuiltinInlineData()
    {
        Console.WriteLine("Creating Evaluation");
        var modelDeploymentName = Environment.GetEnvironmentVariable("DEPLOYMENT_NAME") ?? 
                                  "general-gpt-4.1";
        ArgumentException.ThrowIfNullOrEmpty(modelDeploymentName,
            "Please set the DEPLOYMENT_NAME environment variable to your model deployment name.");
        using var evaluationDataContent = BinaryContent.Create(EvalTestExtensions.GetDataConfig(modelDeploymentName));
        var evaluation = await evaluationClient.CreateEvaluationAsync(evaluationDataContent);
        var fields = EvalTestExtensions.ParseClientResult(evaluation, ["name", "id"]);
        var evaluationName = fields["name"];
        var evaluationId = fields["id"];
        Console.WriteLine($"Evaluation created (id: {evaluationId}, name: {evaluationName})");
        Console.WriteLine("Get Evaluation by Id");
        var evaluationResponse = await evaluationClient.GetEvaluationAsync(evaluationId, new());
        Console.WriteLine($"Retrieved evaluation: {evaluationResponse.GetRawResponse().Content}");
        Console.WriteLine("Creating Eval Run with Inline Data");
        using var runDataContent = BinaryContent.Create(EvalTestExtensions.GetData());
        var run = await evaluationClient.CreateEvaluationRunAsync(evaluationId: evaluationId, content: runDataContent);
        fields = EvalTestExtensions.ParseClientResult(run, ["id", "status"]);
        var runId = fields["id"];
        var runStatus = fields["status"];
        Console.WriteLine($"Evaluation run created (id: {runId})");
        Console.WriteLine("Get Eval Run by Id");
        var evalRunResponse = await evaluationClient.GetEvaluationRunAsync(evaluationId: evaluationId, evaluationRunId: runId, options: new());
        Console.WriteLine($"Eval Run Response: {evalRunResponse.GetRawResponse().Content}");
        
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
        Assert.Pass("Coherence evaluation with inline data completed successfully.");
    }
}