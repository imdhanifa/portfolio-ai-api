using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.MCP;

namespace Portfolio.Api.Controllers;

/// <summary>
/// Exposes the structured portfolio data (profile/skills/projects/experience) as plain
/// REST endpoints, backed by the same MCP tools the AI assistant calls internally.
/// </summary>
[ApiController]
public class PortfolioController(PortfolioMcpServer mcpServer) : ControllerBase
{
    [HttpGet("api/profile")]
    public async Task<ContentResult> GetProfile(CancellationToken cancellationToken) =>
        await ToolResult("get_profile", cancellationToken);

    [HttpGet("api/skills")]
    public async Task<ContentResult> GetSkills(CancellationToken cancellationToken) =>
        await ToolResult("get_skills", cancellationToken);

    [HttpGet("api/projects")]
    public async Task<ContentResult> GetProjects(CancellationToken cancellationToken) =>
        await ToolResult("get_projects", cancellationToken);

    [HttpGet("api/experience")]
    public async Task<ContentResult> GetExperience(CancellationToken cancellationToken) =>
        await ToolResult("get_experience", cancellationToken);

    [HttpGet("api/education")]
    public async Task<ContentResult> GetEducation(CancellationToken cancellationToken) =>
        await ToolResult("get_education", cancellationToken);

    private async Task<ContentResult> ToolResult(string toolName, CancellationToken cancellationToken)
    {
        var json = await mcpServer.CallToolAsync(toolName, cancellationToken) ?? "{}";
        return Content(json, "application/json");
    }
}
