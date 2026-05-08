using System.ClientModel;
using Azure.AI.Projects;
using Azure.AI.Projects.Evaluation;
using Azure.Identity;
using Eval.Tests.Base;
using OpenAI.Evals;

namespace Eval.BuiltIn.EvaluatorsCatalog;

public class EvaluatorsCatalogTests
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
    public async Task TestEvaluatorsCatalog()
    {
        Console.WriteLine("Creating prompt-based evaluator.");
        EvaluatorVersion promptEvaluator = await projectClient.Evaluators.CreateVersionAsync(
            name: "myCustomEvaluatorPromptBased",
            evaluatorVersion: EvaluatorsTestExtensions.GetPromptEvaluatorVersion()
        );
        EvaluatorsTestExtensions.DisplayEvaluatorVersion(promptEvaluator);
        Console.WriteLine("Creating code-based evaluator.");
        EvaluatorVersion codeEvaluator = await projectClient.Evaluators.CreateVersionAsync(
            name: "myCustomEvaluatorCodeBased",
            evaluatorVersion: EvaluatorsTestExtensions.GetCodeEvaluatorVersion()
        );
        EvaluatorsTestExtensions.DisplayEvaluatorVersion(codeEvaluator);
        Console.WriteLine("Get code-based evaluator.");
        EvaluatorVersion codeEvaluatorLatest = await projectClient.Evaluators.GetVersionAsync(name: codeEvaluator.Name, version: codeEvaluator.Version);
        EvaluatorsTestExtensions.DisplayEvaluatorVersion(codeEvaluatorLatest);
        Console.WriteLine("Get prompt-based evaluator.");
        EvaluatorVersion promptEvaluatorLatest = await projectClient.Evaluators.GetVersionAsync(name: promptEvaluator.Name, version: promptEvaluator.Version);
        EvaluatorsTestExtensions.DisplayEvaluatorVersion(promptEvaluatorLatest);
        
        Console.WriteLine("Updating code-based evaluator.");
        var evaluatorVersionUpdate = BinaryData.FromObjectAsJson(
            new
            {
                categories = new[] { EvaluatorCategory.Quality.ToString() },
                display_name = "my_custom_evaluator_updated",
                description = "Custom evaluator description changed"
            }
        );
        using var evaluatorVersionUpdateContent = BinaryContent.Create(evaluatorVersionUpdate);
        var response = await projectClient.Evaluators.UpdateVersionAsync(
            name: codeEvaluator.Name,
            version: codeEvaluator.Version,
            content: evaluatorVersionUpdateContent
        );
        EvaluatorVersion updatedEvaluator = ClientResult.FromValue((EvaluatorVersion)response, response.GetRawResponse());
        EvaluatorsTestExtensions.DisplayEvaluatorVersion(updatedEvaluator);
        Console.WriteLine("Listing built-in evaluators.");
        await foreach (var evaluator in projectClient.Evaluators.GetLatestVersionsAsync(type: ListVersionsRequestType.BuiltIn))
        {
            EvaluatorsTestExtensions.DisplayEvaluatorVersion(evaluator);
        }
        Console.WriteLine("Listing custom evaluators.");
        await foreach (EvaluatorVersion evaluator in projectClient.Evaluators.GetLatestVersionsAsync(type: ListVersionsRequestType.Custom))
        {
            EvaluatorsTestExtensions.DisplayEvaluatorVersion(evaluator);
        }
        Assert.Pass("Evaluators catalog test passed.");
        //cleanup
        // await projectClient.Evaluators.DeleteVersionAsync(name: promptEvaluatorLatest.Name, version: promptEvaluatorLatest.Version);
        // await projectClient.Evaluators.DeleteVersionAsync(name: codeEvaluatorLatest.Name, version: codeEvaluatorLatest.Version);
    }
}