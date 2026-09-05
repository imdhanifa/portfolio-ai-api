using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.AI;
using Portfolio.Api.Models;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("chat")]
public class ChatController(IChatService chatService) : ControllerBase
{
    private const int MaxMessageLength = 2000;

    /// <summary>Ask the AI portfolio assistant a question.</summary>
    [HttpPost]
    [ProducesResponseType<ChatResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("message is required.");
        }

        if (request.Message.Length > MaxMessageLength)
        {
            return BadRequest($"message must be {MaxMessageLength} characters or fewer.");
        }

        var response = await chatService.AskAsync(request, cancellationToken);
        return Ok(response);
    }
}
