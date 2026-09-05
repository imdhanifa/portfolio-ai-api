namespace Portfolio.Api.AI;

/// <summary>Configuration for the xAI Grok API. Bound from the "Grok" section of appsettings.json / env vars.</summary>
public class GrokOptions
{
    public const string SectionName = "Grok";

    /// <summary>Set via the XAI_API_KEY environment variable — never commit this.</summary>
    public string ApiKey { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://api.x.ai/v1";

    /// <summary>
    /// From a working xAI-documented curl example (model "grok-4.6" against POST
    /// /v1/responses) - more trustworthy than the earlier "grok-4-fast" guess, but still
    /// not confirmed end-to-end against a real 200 response (the account has no credits
    /// yet). Override via appsettings/env if this turns out to be stale.
    /// </summary>
    public string Model { get; set; } = "grok-4.6";
}
