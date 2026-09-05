using Portfolio.Api.MCP.Tools;

namespace Portfolio.Api.MCP;

/// <summary>
/// Registry/dispatcher for the portfolio MCP tools (GetProfile, GetSkills, GetProjects,
/// GetExperience). Called directly in-process for now; Phase 5 wires this up behind an
/// actual MCP transport (stdio/HTTP) so external MCP clients can call the same tools.
/// </summary>
public class PortfolioMcpServer(IEnumerable<IPortfolioTool> tools)
{
    private readonly IReadOnlyDictionary<string, IPortfolioTool> _tools =
        tools.ToDictionary(t => t.Name, t => t);

    /// <summary>All registered tools, for listing/discovery.</summary>
    public IReadOnlyCollection<IPortfolioTool> Tools => _tools.Values.ToList();

    /// <summary>Invoke a tool by name (e.g. "get_projects"). Returns null if the tool doesn't exist.</summary>
    public async Task<string?> CallToolAsync(string toolName, CancellationToken cancellationToken = default)
    {
        if (!_tools.TryGetValue(toolName, out var tool))
        {
            return null;
        }

        return await tool.InvokeAsync(cancellationToken);
    }
}
