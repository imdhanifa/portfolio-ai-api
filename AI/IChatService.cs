using Portfolio.Api.Models;

namespace Portfolio.Api.AI;

/// <summary>
/// Orchestrates a single chat turn: decide which MCP context is needed, gather it, build
/// the final prompt, call Grok, and return the answer with its sources.
/// </summary>
public interface IChatService
{
    Task<ChatResponse> AskAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
