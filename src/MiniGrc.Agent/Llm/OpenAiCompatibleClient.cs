using System.Net.Http.Json;
using System.Text.Json;

namespace MiniGrc.Agent.Llm;

/// <summary>Minimal OpenAI-compatible chat client for any <c>/v1/chat/completions</c> endpoint
/// (OpenAI, LM Studio, Ollama). Dependency-free so the agent has no hard vendor coupling.</summary>
public sealed class OpenAiCompatibleClient
{
    private readonly HttpClient _http;
    private readonly string _model;

    public OpenAiCompatibleClient(HttpClient httpClient, string model)
    {
        _http = httpClient;
        _model = model;
    }

    public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, double temperature = 0.2, CancellationToken ct = default)
    {
        var payload = new
        {
            model = _model,
            temperature,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            }
        };

        using var response = await _http.PostAsJsonAsync("v1/chat/completions", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"LLM endpoint returned {(int)response.StatusCode}: {body}");
        }

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
        return content ?? string.Empty;
    }
}

file sealed class ChatResponse
{
    public List<ChatChoice>? choices { get; set; }
    public sealed class ChatChoice
    {
        public ChatMessage? message { get; set; }
    }
    public sealed class ChatMessage
    {
        public string? content { get; set; }
    }
}
