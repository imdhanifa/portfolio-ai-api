using System.Text.Json;
using Microsoft.Extensions.Options;
using Portfolio.Api.MCP;

namespace Portfolio.Api.Resume;

/// <summary>
/// Reads and deserializes the portfolio JSON files (Data/profile.json, skills.json,
/// projects.json, experience.json, education.json) into typed models for the resume PDF.
/// </summary>
public class ResumeDataLoader(IOptions<PortfolioDataOptions> options, ILogger<ResumeDataLoader> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<ResumeData> LoadAsync(CancellationToken cancellationToken = default)
    {
        return new ResumeData
        {
            Profile = await ReadAsync<ProfileData>("profile.json", cancellationToken) ?? new ProfileData(),
            Skills = await ReadSkillsAsync(cancellationToken),
            Experience = await ReadAsync<List<ExperienceData>>("experience.json", cancellationToken) ?? [],
            Projects = await ReadAsync<List<ProjectData>>("projects.json", cancellationToken) ?? [],
            Education = await ReadAsync<List<EducationData>>("education.json", cancellationToken) ?? [],
        };
    }

    private async Task<T?> ReadAsync<T>(string fileName, CancellationToken cancellationToken)
    {
        var path = Path.Combine(options.Value.DataDirectory, fileName);
        if (!File.Exists(path))
        {
            logger.LogWarning("Portfolio data file not found: {Path}", path);
            return default;
        }

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);
    }

    /// <summary>
    /// skills.json is a category -> string[] object. Parsed via JsonDocument (rather than
    /// straight into a Dictionary) to preserve the exact category order from the file -
    /// Dictionary&lt;,&gt; enumeration order isn't a guaranteed contract, and category order
    /// matters for how the resume reads.
    /// </summary>
    private async Task<List<KeyValuePair<string, List<string>>>> ReadSkillsAsync(CancellationToken cancellationToken)
    {
        var path = Path.Combine(options.Value.DataDirectory, "skills.json");
        if (!File.Exists(path))
        {
            logger.LogWarning("Portfolio data file not found: {Path}", path);
            return [];
        }

        await using var stream = File.OpenRead(path);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var result = new List<KeyValuePair<string, List<string>>>();
        foreach (var category in doc.RootElement.EnumerateObject())
        {
            var skills = category.Value.EnumerateArray().Select(v => v.GetString() ?? string.Empty).ToList();
            if (skills.Count > 0)
            {
                result.Add(new KeyValuePair<string, List<string>>(category.Name, skills));
            }
        }

        return result;
    }
}
