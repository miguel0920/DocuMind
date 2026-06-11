using DocuMind.Application.Common;

namespace DocuMind.Infrastructure.Services.Embeddings;

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIEmbeddingService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        // Aquí iría tu llamada HTTP a "api.openai.com/v1/embeddings"
        // Estructura JSON diferente, pero devuelve el mismo float[]
        await Task.CompletedTask;
        return [];
    }
}