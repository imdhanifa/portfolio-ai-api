using Microsoft.Extensions.Options;

namespace Portfolio.Api.MCP.Tools;

/// <summary>MCP tool: GetExperience — returns experience.json (work history entries).</summary>
public class ExperienceTool(IOptions<PortfolioDataOptions> options, ILogger<ExperienceTool> logger)
    : JsonFileTool(options, logger, "experience.json")
{
    public override string Name => "get_experience";
    public override string Description => "Returns the portfolio owner's work experience history.";
}
