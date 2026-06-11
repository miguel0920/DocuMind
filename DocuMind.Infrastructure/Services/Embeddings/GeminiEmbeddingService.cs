using DocuMind.Application.Common;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DocuMind.Infrastructure.Services.Embeddings;

public class GeminiEmbeddingService(HttpClient httpClient, string url) : IEmbeddingService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _url = url;

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];

        var requestBody = new { content = new { parts = new[] { new { text } } } };

        // Construimos la URL dinámica con la API Key

        var response = await _httpClient.PostAsJsonAsync(_url, requestBody, cancellationToken);
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