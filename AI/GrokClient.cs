using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Portfolio.Api.AI;

/// <summary>
/// Calls the xAI Grok Responses API (POST /v1/responses).
/// </summary>
public class GrokClient(IHttpClientFactory httpClientFactory, IOptions<GrokOptions> options, ILogger<GrokClient> logger) : IGrokClient
{
    public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        var config = options.Value;

        if (string.IsNullOrWhiteSpace(config.ApiKey))
        {
            throw new GrokApiException("XAI_API_KEY is not configured.");
        }

        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(config.BaseUrl.TrimEnd('/') + "/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiKey);

        var request = new GrokResponseRequest
        {
            Model = config.Model,
            Input =
            [
                new GrokInputMessage { Role = "system", Content = systemPrompt },
                new GrokInputMessage { Role = "user", Content = userPrompt },
            ],
        };

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync("responses", request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to reach the Grok API.");
            throw new GrokApiException("Failed to reach the Grok API.", ex);
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Grok API returned {StatusCode}: {Body}", response.StatusCode, body);
            throw new GrokApiException($"Grok API returned {(int)response.StatusCode} {response.StatusCode}.");
        }

        var answer = ExtractOutputText(body);
        if (string.IsNullOrWhiteSpace(answer))
        {
            logger.LogError("Grok API returned 2xx but no extractable output_text. Body: {Body}", body);
            throw new GrokApiException("Grok API returned an empty or unrecognized response.");
        }

        return answer;
    }

    /// <summary>
    /// Extracts the assistant's text from a Responses API payload. The exact success shape
    /// hasn't been confirmed against a live (credited) account yet, so this parses
    /// defensively against the documented convention rather than a rigid DTO: prefer a
    /// top-level "output_text" convenience string, else walk output[].content[] for
    /// output_text/text parts. TODO: tighten this to strict DTOs once a real 200 response
    /// body is available to confirm the exact shape.
    /// </summary>
    private string? ExtractOutputText(string responseBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.TryGetProperty("output_text", out var outputTextProp) && outputTextProp.ValueKind == JsonValueKind.String)
            {
                return outputTextProp.GetString();
            }

            if (root.TryGetProperty("output", out var outputProp) && outputProp.ValueKind == JsonValueKind.Array)
            {
                var sb = new System.Text.StringBuilder();
                foreach (var item in outputProp.EnumerateArray())
                {
                    if (!item.TryGetProperty("content", out var contentProp) || contentProp.ValueKind != JsonValueKind.Array)
                    {
                        continue;
                    }

                    foreach (var part in contentProp.EnumerateArray())
                    {
                        var type = part.TryGetProperty("type", out var typeProp) ? typeProp.GetString() : null;
                        if ((type is "output_text" or "text") && part.TryGetProperty("text", out var textProp))
                        {
                            sb.Append(textProp.GetString());
                        }
                    }
                }

                return sb.Length > 0 ? sb.ToString() : null;
            }

            return null;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to parse Grok API response as JSON.");
            return null;
        }
    }
}
