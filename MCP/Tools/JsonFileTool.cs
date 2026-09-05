using Microsoft.Extensions.Options;

namespace Portfolio.Api.MCP.Tools;

/// <summary>
/// Base class for portfolio tools that just read a single JSON file out of the data directory.
/// </summary>
public abstract class JsonFileTool(IOptions<PortfolioDataOptions> options, ILogger logger, string fileName) : IPortfolioTool
{
    private readonly string _filePath = Path.Combine(options.Value.DataDirectory, fileName);

    public abstract string Name { get; }
    public abstract string Description { get; }

    public async Task<string> InvokeAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            logger.LogWarning("Portfolio data file not found: {FilePath}", _filePath);
            return "{}";
        }

        return await File.ReadAllTextAsync(_filePath, cancellationToken);
    }
}
