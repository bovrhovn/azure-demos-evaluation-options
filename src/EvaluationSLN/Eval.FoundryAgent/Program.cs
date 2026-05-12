using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Spectre.Console;

AnsiConsole.MarkupLine("[grey]Foundry Agent for evaluation[/]");

#region Environment variables

var azureFoundryEndpoint = Environment.GetEnvironmentVariable("FOUNDRY_ENDPOINT");
ArgumentException.ThrowIfNullOrEmpty(azureFoundryEndpoint,
    "Please set the FOUNDRY_ENDPOINT environment variable to your Azure OpenAI endpoint URL.");
var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT");
ArgumentException.ThrowIfNullOrEmpty(azureOpenAIEndpoint,
    "Please set the OPENAI_ENDPOINT environment variable to your Azure OpenAI endpoint URL.");
var agentName = Environment.GetEnvironmentVariable("AGENT_NAME");
ArgumentException.ThrowIfNullOrEmpty(agentName,
    "Please set the AGENT_NAME environment variable to your Azure OpenAI endpoint URL.");
var deploymentName = Environment.GetEnvironmentVariable("DEPLOYMENT_NAME");
ArgumentException.ThrowIfNullOrEmpty(deploymentName,
    "Please set the DEPLOYMENT_NAME environment variable to your Azure OpenAI endpoint URL.");

#endregion

var aiProjectClient = new AIProjectClient(new Uri(azureFoundryEndpoint), new DefaultAzureCredential());
ProjectsAgentRecord defaultAgent = await aiProjectClient.AgentAdministrationClient
    .GetAgentAsync(agentName);
var agent = aiProjectClient.AsAIAgent(defaultAgent);
AnsiConsole.MarkupLine(
    $"[green]Successfully retrieved agent with name {agentName} and id {agent.Id}[/]. " +
    $"Asking question now.");
var question = AnsiConsole.Ask<string>("Ask your question",
    "What is the 2nd largest city in Poland by population size?");
AnsiConsole.MarkupLine("[green]Question:[/]" + question);
var agentMessage = await agent.RunAsync(question);
AnsiConsole.MarkupLine("[green]Answer:[/]");
AnsiConsole.WriteLine(agentMessage.Text);
AnsiConsole.WriteLine("----------------------------------------------");

var continueWith = AnsiConsole.Ask("Continue with Local evaluator?",
    true);
if (continueWith)
{
    //create local evaluator
    var evaluator = new LocalEvaluator(
        FunctionEvaluator.Create("coherence", item =>
        {
            //do check manually
            var response = item.Response;
            AnsiConsole.WriteLine($"Evaluating response: {response}");
            return new EvalCheckResult(response.Contains("Krakow"),
                "Checking coherence eval Krakow",
                "Local coherence check");
        })
    );
    //get the eval from the run
    await agent.EvaluateAsync([question], evaluator);
}

continueWith = AnsiConsole.Ask("Continue with Azure Coherence evaluator?",
    true);
var client =
    new ChatClientBuilder(
        new AzureOpenAIClient(new Uri(azureOpenAIEndpoint), new DefaultAzureCredential())
            .GetChatClient(deploymentName)
            .AsIChatClient())
        .Build();
var chatConfiguration = new ChatConfiguration(client);
AnsiConsole.MarkupLine($"[grey]Chat client with Azure OpenAI model {deploymentName} created[/]");
if (continueWith)
{
    //check with Azure Foundry evaluator 
    var coherenceEvaluator = new CoherenceEvaluator();
    var result = await coherenceEvaluator.EvaluateAsync(question, agentMessage.Text,
        chatConfiguration);
    foreach (var keyValuePair in result.Metrics)
    {
        AnsiConsole.WriteLine($"Metric: {keyValuePair.Key}");
        AnsiConsole.WriteLine($"Value: {keyValuePair.Value}");
    }
}

continueWith = AnsiConsole.Ask("Continue with multiple evaluators?",
    true);
if (continueWith)
{
    //check with composite evaluator - multiple evals
    var evaluator = new CompositeEvaluator(
        new RelevanceEvaluator(),
        new FluencyEvaluator()
    );

    var result = await evaluator.EvaluateAsync(question, agentMessage.Text, 
        chatConfiguration);
    foreach (var keyValuePair in result.Metrics)
    {
        AnsiConsole.WriteLine($"Metric: {keyValuePair.Key}");
        AnsiConsole.WriteLine($"Value: {keyValuePair.Value}");
    }
}