using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Spectre.Console;

AnsiConsole.MarkupLine("[grey]Foundry Agent for evaluation[/]");

#region Environment variables

var endpoint = Environment.GetEnvironmentVariable("ENDPOINT");
ArgumentException.ThrowIfNullOrEmpty(endpoint,
    "Please set the ENDPOINT environment variable to your Azure OpenAI endpoint URL.");
var agentName = Environment.GetEnvironmentVariable("AGENT_NAME");
ArgumentException.ThrowIfNullOrEmpty(agentName,
    "Please set the AGENT_NAME environment variable to your Azure OpenAI endpoint URL.");

#endregion

var aiProjectClient = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential());
ProjectsAgentRecord defaultAgent = await aiProjectClient.AgentAdministrationClient.GetAgentAsync(agentName);
var agent = aiProjectClient.AsAIAgent(defaultAgent);
AnsiConsole.MarkupLine(
    $"[green]Successfully retrieved agent with name {agentName} and id {agent.Id}[/]. Asking question now.");
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
if (continueWith)
{
    //check with Azure Foundry evaluator 
    var coherenceEvaluator = new CoherenceEvaluator();
    var result = await coherenceEvaluator.EvaluateAsync(question, agentMessage.Text);
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

    var result = await evaluator.EvaluateAsync(question, agentMessage.Text);
    foreach (var keyValuePair in result.Metrics)
    {
        AnsiConsole.WriteLine($"Metric: {keyValuePair.Key}");
        AnsiConsole.WriteLine($"Value: {keyValuePair.Value}");
    }
}