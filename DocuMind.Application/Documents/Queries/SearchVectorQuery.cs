using DocuMind.Application.Common;
using DocuMind.Application.DTOs;
using DocuMind.Application.Persistence;
using MediatR;
using Microsoft.Extensions.Configuration;
using Pgvector;
using System.Text.Json;

namespace DocuMind.Application.Documents.Queries;

public record SearchVectorQuery(string QueryText, int Limit = 3) : IRequest<RagResponseDto>;

public class SearchVectorQueryHandler(IEmbeddingService embeddingService,
                                      IDocumentRepository documentRepository,
                                      IChatService chatService,
                                      IConfiguration configuration) : IRequestHandler<SearchVectorQuery, RagResponseDto>
{
    public async Task<RagResponseDto> Handle(SearchVectorQuery request, CancellationToken cancellationToken)
    {
        var emptyResponse = new RagResponseDto { Response = "Por favor, ingresa una pregunta válida." };

        if (string.IsNullOrWhiteSpace(request.QueryText)) return emptyResponse;

        float[] queryEmbeddingResult = await embeddingService.GenerateEmbeddingAsync(request.QueryText, cancellationToken);
        var queryVector = new Vector(queryEmbeddingResult);

        var similarChunks = await documentRepository.SearchHybridAsync(queryVector, request.QueryText, request.Limit, cancellationToken);

        if (!similarChunks.Any())
        {
            return new RagResponseDto { Response = "No encontré información relevante en los documentos." };
        }

        var contextText = string.Join("\n\n", similarChunks.Select(c => $"- [FUENTE: {c.DocumentName}]: {c.Content}"));

        string systemPromptTemplate = configuration["PromptSettings:SystemPrompt"]
            ?? "Contexto:\n{context}\nPregunta:\n{query}";

        string finalPrompt = systemPromptTemplate
            .Replace("{context}", contextText)
            .Replace("{query}", request.QueryText);

        string aiResponse = await chatService.GenerateResponseAsync(finalPrompt, cancellationToken);

        var cleanJson = aiResponse.Replace("```json", "").Replace("```", "").Trim();

        try
        {
            var finalResult = JsonSerializer.Deserialize<RagResponseDto>(cleanJson);

            if (finalResult != null && finalResult.Sources != null)
            {
                finalResult.Sources = [.. finalResult.Sources.Distinct()];
            }

            return finalResult ?? new RagResponseDto { Response = "Error procesando la respuesta." };
        }
        catch (JsonException)
        {
            return new RagResponseDto
            {
                Response = aiResponse,
                Sources = ["No se pudieron extraer las citas correctamente."]
            };
        }
    }
}