namespace Portfolio.Api;

/// <summary>
/// Rate limiting configuration, bound from the "RateLimiting" section of appsettings.json.
/// Two policies:
///  - "chat": applied to POST /api/chat, which triggers a real (billed) Grok API call.
///  - "global": a looser fallback applied to every other endpoint as a general safety net.
/// Both are partitioned per client IP with a fixed window.
/// </summary>
public class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public RateLimitPolicyOptions Chat { get; set; } = new() { PermitLimit = 10, WindowSeconds = 60 };

    public RateLimitPolicyOptions Global { get; set; } = new() { PermitLimit = 100, WindowSeconds = 60 };
}

public class RateLimitPolicyOptions
{
    /// <summary>Max requests allowed per client IP within the window.</summary>
    public int PermitLimit { get; set; }

    /// <summary>Length of the fixed window, in seconds.</summary>
    public int WindowSeconds { get; set; }
}
