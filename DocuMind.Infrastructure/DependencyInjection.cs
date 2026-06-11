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

        // 1. Registramos un HttpClient genérico para que lo compartan los servicios
        services.AddHttpClient();

        // 2. Registramos la fábrica que actuará como nuestro "Switch"
        services.AddScoped<IEmbeddingService>(provider =>
        {
            var httpClient = provider.GetRequiredService<HttpClient>();

            // Leemos las variables del appsettings.json
            string providerName = configuration["EmbeddingSettings:Provider"] ?? "Gemini";
            string apiKey = configuration["EmbeddingSettings:ApiKey"] ?? string.Empty;

            // Evaluamos cuál servicio entregar según la configuración
            return providerName.ToLower() switch
            {
                "gemini" => new GeminiEmbeddingService(httpClient, apiKey),
                "openai" => new OpenAIEmbeddingService(httpClient, apiKey),
                _ => throw new NotImplementedException($"El proveedor de IA '{providerName}' no está soportado.")
            };
        });

        return services;
    }
}