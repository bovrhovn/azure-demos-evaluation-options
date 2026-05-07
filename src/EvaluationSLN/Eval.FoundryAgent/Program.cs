using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.Identity;
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
    "What is the 2nd largest city in United States by population size?");
AnsiConsole.MarkupLine("[green]Question:[/]" + question);
var agentMessage = await agent.RunAsync(question);
AnsiConsole.MarkupLine("[green]Answer:[/]");
AnsiConsole.WriteLine(agentMessage.Text);