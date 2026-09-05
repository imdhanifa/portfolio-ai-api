using System.Text.Json.Serialization;

namespace Portfolio.Api.AI;

// DTOs for xAI's Responses API - POST /v1/responses (an "input" array of role/content
// messages, not the older OpenAI-style "messages" chat/completions shape).
// Confirmed live: a request in this shape reaches the endpoint and is billing-gated
// (403 permission-denied for an account with no credits), not 404/format-rejected.

public class GrokInputMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class GrokResponseRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("input")]
    public List<GrokInputMessage> Input { get; set; } = [];
}

/// <summary>Thrown when the Grok API call fails (auth, quota, network, unexpected response shape).</summary>
public class GrokApiException(string message, Exception? inner = null) : Exception(message, inner);
