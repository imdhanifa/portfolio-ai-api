using Microsoft.Extensions.Options;

namespace Portfolio.Api.MCP.Tools;

/// <summary>MCP tool: GetProfile — returns profile.json (name, title, summary).</summary>
public class ProfileTool(IOptions<PortfolioDataOptions> options, ILogger<ProfileTool> logger)
    : JsonFileTool(options, logger, "profile.json")
{
    public override string Name => "get_profile";
    public override string Description => "Returns the portfolio owner's name, professional title and summary.";
}
