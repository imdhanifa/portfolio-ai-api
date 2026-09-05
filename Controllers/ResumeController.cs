using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Resume;

namespace Portfolio.Api.Controllers;

[ApiController]
public class ResumeController(ResumePdfService resumePdfService) : ControllerBase
{
    /// <summary>Generates an ATS-friendly resume PDF from the current portfolio data and returns it as a download.</summary>
    [HttpGet("api/resume/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPdf(CancellationToken cancellationToken)
    {
        var result = await resumePdfService.GenerateAsync(cancellationToken);
        return File(result.Bytes, "application/pdf", result.FileName);
    }
}
