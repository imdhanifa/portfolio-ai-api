using System.Text.RegularExpressions;
using QuestPDF.Fluent;

namespace Portfolio.Api.Resume;

public record ResumePdfResult(byte[] Bytes, string FileName);

/// <summary>Generates the resume PDF on demand from the current portfolio data.</summary>
public partial class ResumePdfService(ResumeDataLoader dataLoader, ILogger<ResumePdfService> logger)
{
    public async Task<ResumePdfResult> GenerateAsync(CancellationToken cancellationToken = default)
    {
        var data = await dataLoader.LoadAsync(cancellationToken);
        logger.LogInformation(
            "Generating resume PDF: {SkillCategories} skill categories, {ExperienceCount} experience entries, {ProjectCount} projects.",
            data.Skills.Count, data.Experience.Count, data.Projects.Count);

        var document = new ResumeDocument(data);
        var bytes = document.GeneratePdf();

        return new ResumePdfResult(bytes, BuildFileName(data.Profile.Name));
    }

    private static string BuildFileName(string name)
    {
        var slug = NonAlphaNumeric().Replace(name, "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "Resume.pdf" : $"{slug}-Resume.pdf";
    }

    [GeneratedRegex(@"[^A-Za-z0-9]+")]
    private static partial Regex NonAlphaNumeric();
}
