using Microsoft.Extensions.Options;

namespace Portfolio.Api.MCP.Tools;

/// <summary>MCP tool: GetEducation — returns education.json (school, degree, dates).</summary>
public class EducationTool(IOptions<PortfolioDataOptions> options, ILogger<EducationTool> logger)
    : JsonFileTool(options, logger, "education.json")
{
    public override string Name => "get_education";
    public override string Description => "Returns the portfolio owner's education history.";
}
