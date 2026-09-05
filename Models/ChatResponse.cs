namespace Portfolio.Api.Models;

/// <summary>
/// Response body for POST /api/chat.
/// </summary>
public class ChatResponse
{
    /// <summary>The assistant's answer.</summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>Sources used to ground the answer (e.g. "resume.pdf", "projects.json").</summary>
    public List<string> Sources { get; set; } = [];
}
