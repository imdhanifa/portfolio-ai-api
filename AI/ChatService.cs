using Portfolio.Api.MCP;
using Portfolio.Api.Models;

namespace Portfolio.Api.AI;

/// <summary>
/// Chat orchestrator. Wires MCP + prompt building + Grok together. Grok is the project's
/// only LLM provider - if it fails for any reason (no API credits, outage, not configured),
/// this falls back to a placeholder answer built from whatever MCP tool data it can gather.
/// Keeps POST /api/chat resilient and testable no matter what state the Grok account is in.
/// </summary>
public class ChatService(
    PortfolioMcpServer mcpServer,
    IPromptService promptService,
    IGrokClient grokClient,
    ILogger<ChatService> logger) : IChatService
{
    public async Task<ChatResponse> AskAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var sources = new List<string>();

        // TODO (Phase 6): replace this keyword heuristic with proper LLM-driven tool
        // selection (Grok function calling) as described in the AI Decision Flow.
        var mcpResults = new Dictionary<string, string>();
        foreach (var tool in SelectRelevantTools(request.Message))
        {
            var result = await mcpServer.CallToolAsync(tool.Name, cancellationToken);
            if (result is not null)
            {
                mcpResults[tool.Name] = result;
                sources.Add($"{tool.Name}.json");
            }
        }

        var systemPrompt = promptService.BuildSystemPrompt();
        var userPrompt = promptService.BuildUserPrompt(request.Message, mcpResults);

        try
        {
            var answer = await grokClient.CompleteAsync(systemPrompt, userPrompt, cancellationToken);
            return new ChatResponse { Answer = answer, Sources = sources };
        }
        catch (Exception ex) when (ex is NotImplementedException or GrokApiException)
        {
            logger.LogWarning(ex, "Grok call failed or is not wired up; returning a placeholder chat response.");
            return BuildPlaceholderResponse(mcpResults, sources);
        }
    }

    private IEnumerable<MCP.Tools.IPortfolioTool> SelectRelevantTools(string message)
    {
        var lower = message.ToLowerInvariant();
        var tools = mcpServer.Tools;

        if (lower.Contains("project")) yield return tools.First(t => t.Name == "get_projects");
        if (lower.Contains("skill") || lower.Contains("technolog")) yield return tools.First(t => t.Name == "get_skills");
        if (lower.Contains("experience") || lower.Contains("work")) yield return tools.First(t => t.Name == "get_experience");
        if (lower.Contains("who are you") || lower.Contains("about") || lower.Contains("profile")) yield return tools.First(t => t.Name == "get_profile");
    }

    private static ChatResponse BuildPlaceholderResponse(Dictionary<string, string> mcpResults, List<string> sources)
    {
        var answer = mcpResults.Count > 0
            ? $"(The AI assistant is temporarily unavailable — check server logs for the Grok API error.) Found structured data from: {string.Join(", ", mcpResults.Keys)}."
            : "(The AI assistant is temporarily unavailable — check server logs for the Grok API error.) No matching structured data found either.";

        return new ChatResponse { Answer = answer, Sources = sources };
    }
}
