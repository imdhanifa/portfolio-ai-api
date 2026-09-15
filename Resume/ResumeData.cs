namespace Portfolio.Api.Resume;

// Typed shapes matching Data/profile.json, skills.json, projects.json, experience.json,
// education.json - deserialized case-insensitively, so these plain PascalCase properties
// line up with the camelCase JSON without needing [JsonPropertyName] on every field.

public class ProfileData
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Github { get; set; }
    public string? Website { get; set; }
    public string? Linkedin { get; set; }
}

public class ProjectData
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = [];
    public string? Link { get; set; }
}

public class ExperienceData
{
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = [];
}

public class EducationData
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
}

/// <summary>Everything the resume document needs, gathered in one place.</summary>
public class ResumeData
{
    public ProfileData Profile { get; set; } = new();

    /// <summary>Category name (e.g. "backend") -> skills in that category, in the order they appear in skills.json.</summary>
    public List<KeyValuePair<string, List<string>>> Skills { get; set; } = [];

    public List<ExperienceData> Experience { get; set; } = [];
    public List<ProjectData> Projects { get; set; } = [];
    public List<EducationData> Education { get; set; } = [];
}
