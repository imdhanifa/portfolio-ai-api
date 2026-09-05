namespace Portfolio.Api.AI;

/// <summary>
/// Builds the final prompt sent to Grok from the system rules, MCP tool results and the
/// user's question.
/// </summary>
public interface IPromptService
{
    /// <summary>The fixed system prompt encoding the assistant's rules (see AI System Rules).</summary>
    string BuildSystemPrompt();

    /// <summary>
    /// Combine MCP tool output and the user question into the user-turn content sent to
    /// the model.
    /// </summary>
    string BuildUserPrompt(string question, IReadOnlyDictionary<string, string> mcpResults);
}
