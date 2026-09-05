using Microsoft.Extensions.Options;

namespace Portfolio.Api.MCP.Tools;

/// <summary>MCP tool: GetProjects — returns projects.json (list of portfolio projects).</summary>
public class ProjectsTool(IOptions<PortfolioDataOptions> options, ILogger<ProjectsTool> logger)
    : JsonFileTool(options, logger, "projects.json")
{
    public override string Name => "get_projects";
    public override string Description => "Returns the portfolio owner's projects, including technologies used.";
}
