using DocuMind.Application.Common;
using DocuMind.Application.Persistence;
using DocuMind.Infrastructure.Persistence.Contexts;
using DocuMind.Infrastructure.Repositories;
using DocuMind.Infrastructure.Services.Embeddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocuMind.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
                                  ?? throw new ArgumentNullException("La cadena de conexión 'DefaultConnection' no está configurada.");

        services.AddDbContext<DocuMindDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.UseVector())); // <- UseVector habilita el mapeo matemático nativo

        // 2. Registro del Repositorio
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        string apiKey = configuration["EmbeddingSettings:ApiKey"] ?? throw new ArgumentNullException("API Key no configurada.");
        string baseUrl = configuration["EmbeddingSettings:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1beta";
        string embeddingModel = configuration["EmbeddingSettings:EmbeddingModel"] ?? "text-embedding-004";
        string chatModel = configuration["EmbeddingSettings:ChatModel"] ?? "gemini-2.5-flash";
        string providerName = configuration["EmbeddingSettings:Provider"] ?? "gemini";
        string outputDimensionality = configuration["EmbeddingSettings:OutputDimensionality"] ?? "768";

        // 1. Registramos un HttpClient genérico para que lo compartan los servicios
        services.AddHttpClient();

        // 2. Registramos la fábrica que actuará como nuestro "Switch"
        services.AddScoped<IEmbeddingService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();

            // Leemos las variables del appsettings.json
            string fullUrl = $"{baseUrl}/models/{embeddingModel}:embedContent";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

            // Evaluamos cuál servicio entregar según la configuración
            return providerName.ToLower() switch
            {
                "gemini" => new GeminiEmbeddingService(httpClient, fullUrl, $"models/{embeddingModel}", outputDimensionality),
                "openai" => new OpenAIEmbeddingService(httpClient, fullUrl),
                _ => throw new NotImplementedException($"El proveedor de IA '{providerName}' no está soportado.")
            };
        });

        services.AddScoped<IChatService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();

            string fullUrl = $"{baseUrl}/models/{chatModel}:generateContent";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

            return providerName.ToLower() switch
            {
                "gemini" => new GeminiChatService(httpClient, fullUrl),
                // En el futuro: "openai" => new OpenAIChatService(httpClient, apiKey),
                _ => throw new NotImplementedException($"El proveedor de Chat '{providerName}' no está soportado.")
            };
        });

        return services;
    }
}