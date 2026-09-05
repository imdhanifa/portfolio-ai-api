namespace Portfolio.Api.Models;

/// <summary>
/// Request body for POST /api/chat.
/// </summary>
public class ChatRequest
{
    /// <summary>The user's question for the AI portfolio assistant.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Optional conversation id, if the client wants multi-turn context tracked server-side.</summary>
    public string? ConversationId { get; set; }
}
