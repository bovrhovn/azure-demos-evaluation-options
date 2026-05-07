using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Spectre.Console;

AnsiConsole.MarkupLine("[grey]MAF Agent for evaluation[/]");

var endpoint = Environment.GetEnvironmentVariable("ENDPOINT");
ArgumentException.ThrowIfNullOrEmpty(endpoint, "Please set the ENDPOINT environment variable to your Azure OpenAI endpoint URL.");
var deploymentName = Environment.GetEnvironmentVariable("DEPLOYMENT_NAME");
ArgumentException.ThrowIfNullOrEmpty(deploymentName, "Please set the DEPLOYMENT_NAME environment variable to your Azure OpenAI endpoint URL.");

AIAgent agent = new AIProjectClient(new Uri(endpoint), new DefaultAzureCredential())
    .AsAIAgent(
        model: deploymentName,
        instructions: "You are a friendly assistant. Keep your answers brief.",
        name: "SimpleAgent");
var question = AnsiConsole.Ask<string>("Ask your question",
    "What is the 2nd largest city in United States by population size?");
AnsiConsole.MarkupLine("[green]Question:[/]" + question);
var answer = await agent.RunAsync(question);
AnsiConsole.MarkupLine("[green]Answer:[/]" + answer);