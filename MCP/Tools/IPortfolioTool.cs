namespace Portfolio.Api.MCP.Tools;

/// <summary>
/// A single MCP-exposed portfolio tool. Each implementation reads one structured JSON
/// file from the data directory and returns it as raw JSON text.
/// </summary>
public interface IPortfolioTool
{
    /// <summary>Tool name as exposed over MCP (e.g. "get_profile").</summary>
    string Name { get; }

    /// <summary>Short description shown to the LLM/tool caller.</summary>
    string Description { get; }

    /// <summary>Reads and returns the underlying JSON file's contents.</summary>
    Task<string> InvokeAsync(CancellationToken cancellationToken = default);
}
