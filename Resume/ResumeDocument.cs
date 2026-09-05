using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Portfolio.Api.Resume;

/// <summary>
/// Composes an ATS-friendly resume PDF from structured portfolio data: single column
/// (no tables/multi-column layout that can scramble ATS text-extraction order), real
/// selectable text throughout (no images), standard section headings, plain bullet lists.
/// </summary>
public class ResumeDocument(ResumeData data) : IDocument
{
    private const string FontFamily = "DejaVu Sans";

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.5f, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontFamily(FontFamily).FontSize(10).FontColor(Colors.Black));

            page.Content().Column(column =>
            {
                column.Spacing(10);

                column.Item().Element(ComposeHeader);

                if (!string.IsNullOrWhiteSpace(data.Profile.Summary))
                {
                    column.Item().Element(c => ComposeSection(c, "PROFESSIONAL SUMMARY", ComposeSummary));
                }

                if (data.Skills.Count > 0)
                {
                    column.Item().Element(c => ComposeSection(c, "TECHNICAL SKILLS", ComposeSkills));
                }

                if (data.Experience.Count > 0)
                {
                    column.Item().Element(c => ComposeSection(c, "PROFESSIONAL EXPERIENCE", ComposeExperience));
                }

                if (data.Projects.Count > 0)
                {
                    column.Item().Element(c => ComposeSection(c, "PROJECTS", ComposeProjects));
                }

                if (data.Education.Count > 0)
                {
                    column.Item().Element(c => ComposeSection(c, "EDUCATION", ComposeEducation));
                }
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().Text(data.Profile.Name).FontSize(20).Bold();

            if (!string.IsNullOrWhiteSpace(data.Profile.Title))
            {
                column.Item().Text(data.Profile.Title).FontSize(12).FontColor(Colors.Grey.Darken2);
            }

            var contactParts = new[] { data.Profile.Location, data.Profile.Phone, data.Profile.Email, data.Profile.Website, data.Profile.Github, data.Profile.Linkedin }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            var contactLine = string.Join("  |  ", contactParts);

            if (!string.IsNullOrWhiteSpace(contactLine))
            {
                column.Item().Text(contactLine).FontSize(9).FontColor(Colors.Grey.Darken1);
            }
        });
    }

    /// <summary>Standard heading + a thin rule, then the section's own content - the same
    /// shape ATS parsers expect ("PROFESSIONAL EXPERIENCE", "EDUCATION", etc.).</summary>
    private void ComposeSection(IContainer container, string heading, Action<IContainer> content)
    {
        container.Column(column =>
        {
            column.Spacing(4);

            column.Item().Text(heading).FontSize(11).Bold().LetterSpacing(0.05f);
            column.Item().LineHorizontal(0.75f).LineColor(Colors.Grey.Lighten1);
            column.Item().Element(content);
        });
    }

    private void ComposeSummary(IContainer container)
    {
        container.Text(data.Profile.Summary).FontSize(10).LineHeight(1.3f);
    }

    private void ComposeSkills(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(3);

            foreach (var (category, skills) in data.Skills)
            {
                column.Item().Text(text =>
                {
                    text.Span(FormatCategoryLabel(category) + ": ").Bold();
                    text.Span(string.Join(", ", skills));
                });
            }
        });
    }

    private void ComposeExperience(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);

            foreach (var entry in data.Experience)
            {
                column.Item().Column(entryColumn =>
                {
                    entryColumn.Spacing(1);

                    entryColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"{entry.Role} — {entry.Company}").Bold();
                        row.ConstantItem(140).AlignRight().Text($"{entry.StartDate} – {entry.EndDate}").FontColor(Colors.Grey.Darken1);
                    });

                    if (!string.IsNullOrWhiteSpace(entry.Location))
                    {
                        entryColumn.Item().Text(entry.Location).FontSize(9).FontColor(Colors.Grey.Darken1);
                    }

                    foreach (var bullet in SplitIntoBullets(entry.Description))
                    {
                        entryColumn.Item().Element(c => ComposeBullet(c, bullet));
                    }

                    if (entry.Technologies.Count > 0)
                    {
                        entryColumn.Item().PaddingTop(2).Text(text =>
                        {
                            text.Span("Technologies: ").Bold().FontSize(9);
                            text.Span(string.Join(", ", entry.Technologies)).FontSize(9);
                        });
                    }
                });
            }
        });
    }

    private void ComposeProjects(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(8);

            foreach (var project in data.Projects)
            {
                column.Item().Column(entryColumn =>
                {
                    entryColumn.Spacing(1);

                    entryColumn.Item().Text(project.Name).Bold();
                    entryColumn.Item().Text(project.Description).FontSize(10).LineHeight(1.3f);

                    if (project.Technologies.Count > 0)
                    {
                        entryColumn.Item().Text(text =>
                        {
                            text.Span("Technologies: ").Bold().FontSize(9);
                            text.Span(string.Join(", ", project.Technologies)).FontSize(9);
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(project.Link))
                    {
                        entryColumn.Item().Text(project.Link).FontSize(9).FontColor(Colors.Blue.Darken1);
                    }
                });
            }
        });
    }

    private void ComposeEducation(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(6);

            foreach (var entry in data.Education)
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"{entry.Degree} — {entry.Institution}").Bold();
                    row.ConstantItem(140).AlignRight().Text($"{entry.StartDate} – {entry.EndDate}").FontColor(Colors.Grey.Darken1);
                });
            }
        });
    }

    private static void ComposeBullet(IContainer container, string text)
    {
        container.Row(row =>
        {
            row.ConstantItem(10).Text("•");
            row.RelativeItem().Text(text).FontSize(10).LineHeight(1.25f);
        });
    }

    /// <summary>Descriptions are written as "Did X; did Y; did Z" - split into individual
    /// bullet points, the standard resume format (and more ATS-friendly than one dense
    /// paragraph). Each fragment is normalized to look like its own sentence: capitalized
    /// start, no trailing period (the mid-sentence ones lowercase-start from the original
    /// clause; the last one carries the original sentence's period) - otherwise splitting
    /// produces "implemented RBAC..." next to "...validation." in the same bullet list.</summary>
    private static IEnumerable<string> SplitIntoBullets(string description) =>
        description
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => s.Length > 0)
            .Select(NormalizeBullet);

    private static string NormalizeBullet(string bullet)
    {
        var trimmed = bullet.TrimEnd('.').Trim();
        return trimmed.Length > 0 ? char.ToUpperInvariant(trimmed[0]) + trimmed[1..] : trimmed;
    }

    private static string FormatCategoryLabel(string category) => category switch
    {
        "ai" => "AI",
        "devops" => "DevOps",
        "packages_and_libraries" => "Packages & Libraries",
        "deployment_and_hosting" => "Deployment & Hosting",
        "development_tools" => "Development Tools",
        // Falls through for every other category.json key (languages, frontend, backend,
        // databases, architecture, security, ...) - title-cases each underscore-separated
        // word instead of assuming a single word, so a new snake_case category picks up a
        // reasonable label without needing its own case added here.
        _ => string.Join(" ", category.Split('_').Select(word => char.ToUpperInvariant(word[0]) + word[1..])),
    };
}
