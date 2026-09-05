using Microsoft.Extensions.Options;

namespace Portfolio.Api.MCP.Tools;

/// <summary>MCP tool: GetSkills — returns skills.json (backend/frontend/database/ai skill lists).</summary>
public class SkillsTool(IOptions<PortfolioDataOptions> options, ILogger<SkillsTool> logger)
    : JsonFileTool(options, logger, "skills.json")
{
    public override string Name => "get_skills";
    public override string Description => "Returns the portfolio owner's technical skills grouped by category.";
}
