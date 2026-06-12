using DocuMind.Application.Common;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DocuMind.Infrastructure.Services.Embeddings;

public class GeminiEmbeddingService(HttpClient httpClient, string url, string modelName, string outputDimensionality) : IEmbeddingService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _url = url;
    private readonly string _modelName = modelName;

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];

        var requestBody = new GeminiEmbeddingRequest
        {
            Model = _modelName, // <- Agregamos el modelo al JSON
            Content = new GeminiContent { Parts = [new GeminiPart { Text = text }] },
            Output_Dimensionality = int.Parse(outputDimensionality)
        };

        // Construimos la URL dinámica con la API Key

        var response = await _httpClient.PostAsJsonAsync(_url, requestBody, cancellationToken);
        if (!response.IsSuccessStatusCode) return [];

        var result = await response.Content.ReadFromJsonAsync<GeminiEmbeddingResponse>(cancellationToken: cancellationToken);
        return result?.Embedding?.Values ?? [];
    }

    public class GeminiEmbeddingRequest
    {
        [JsonPropertyName("model")] // <- Mapeo del nuevo parámetro
        public string? Model { get; set; }

        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }

        [JsonPropertyName("output_dimensionality")]
        public int Output_Dimensionality { get; set; }
    }

    public class GeminiContent
    {
        [JsonPropertyName("parts")]
        public GeminiPart[] Parts { get; set; } = [];
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
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