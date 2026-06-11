using DocuMind.Application.Common;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DocuMind.Infrastructure.Services.Embeddings;

public class GeminiChatService(HttpClient httpClient, string url) : IChatService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _url = url;

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt)) return "Prompt vacío.";

        // Estructura JSON estándar que solicita la API de chat de Gemini
        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var response = await _httpClient.PostAsJsonAsync(_url, requestBody, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return $"Error en el servicio de Chat: {response.ReasonPhrase}";

        var result = await response.Content.ReadFromJsonAsync<GeminiChatResponse>(cancellationToken: cancellationToken);

        // Extraemos el texto de la respuesta de la estructura jerárquica de Google
        return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
               ?? "No se pudo generar una respuesta.";
    }
}

// Clases de utilidad para deserializar la respuesta de Chat de Gemini
public class GeminiChatResponse
{
    [JsonPropertyName("candidates")] public List<GeminiCandidate>? Candidates { get; set; }
}
public class GeminiCandidate
{
    [JsonPropertyName("content")] public GeminiChatContent? Content { get; set; }
}
public class GeminiChatContent
{
    [JsonPropertyName("parts")] public List<GeminiChatPart>? Parts { get; set; }
}
public class GeminiChatPart
{
    [JsonPropertyName("text")] public string? Text { get; set; }
}