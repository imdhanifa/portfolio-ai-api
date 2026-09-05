namespace Portfolio.Api.AI;

/// <summary>
/// Thin client for the xAI Grok chat completions API.
/// </summary>
public interface IGrokClient
{
    Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);
}
