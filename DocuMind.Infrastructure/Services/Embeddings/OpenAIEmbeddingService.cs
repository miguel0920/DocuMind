using DocuMind.Application.Common;

namespace DocuMind.Infrastructure.Services.Embeddings;

public class OpenAIEmbeddingService(HttpClient httpClient, string url) : IEmbeddingService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _url = url;

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        // Aquí iría tu llamada HTTP a "api.openai.com/v1/embeddings"
        // Estructura JSON diferente, pero devuelve el mismo float[]
        await Task.CompletedTask;
        return [];
    }
}