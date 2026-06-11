using DocuMind.Application.Common;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DocuMind.Infrastructure.Services.Embeddings;

public class GeminiEmbeddingService(HttpClient httpClient, string apiKey) : IEmbeddingService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = apiKey;

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];

        var requestBody = new { content = new { parts = new[] { new { text = text } } } };

        // Construimos la URL dinámica con la API Key
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent?key={_apiKey}";

        var response = await _httpClient.PostAsJsonAsync(url, requestBody, cancellationToken);
        if (!response.IsSuccessStatusCode) return [];

        var result = await response.Content.ReadFromJsonAsync<GeminiEmbeddingResponse>(cancellationToken: cancellationToken);
        return result?.Embedding?.Values ?? [];
    }

    public class GeminiEmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public GeminiVector? Embedding { get; set; }
    }

    public class GeminiVector
    {
        [JsonPropertyName("values")]
        public float[]? Values { get; set; }
    }
}