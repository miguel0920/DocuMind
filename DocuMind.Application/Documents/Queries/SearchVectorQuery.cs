using DocuMind.Application.Common;
using DocuMind.Application.Persistence;
using MediatR;
using Microsoft.Extensions.Configuration;
using Pgvector;

namespace DocuMind.Application.Documents.Queries;

public record SearchVectorQuery(string QueryText, int Limit = 3) : IRequest<string>;

public class SearchVectorQueryHandler(IEmbeddingService embeddingService,
                                      IDocumentRepository documentRepository,
                                      IChatService chatService,
                                      IConfiguration configuration) : IRequestHandler<SearchVectorQuery, string>
{
    public async Task<string> Handle(SearchVectorQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.QueryText))
            return "Por favor, ingresa una pregunta válida.";

        float[] queryEmbeddingResult = await embeddingService.GenerateEmbeddingAsync(request.QueryText, cancellationToken);
        var queryVector = new Vector(queryEmbeddingResult);

        var similarChunks = await documentRepository.SearchHybridAsync(queryVector, request.QueryText, request.Limit, cancellationToken);

        if (!similarChunks.Any())
            return "No se encontró información relevante en los documentos guardados.";

        var contextText = string.Join("\n\n", similarChunks.Select(c => $"- [{c.DocumentName}]: {c.Content}"));

        string systemPromptTemplate = configuration["PromptSettings:SystemPrompt"]
            ?? "Contexto:\n{context}\nPregunta:\n{query}";

        string finalPrompt = systemPromptTemplate
            .Replace("{context}", contextText)
            .Replace("{query}", request.QueryText);

        string aiResponse = await chatService.GenerateResponseAsync(finalPrompt, cancellationToken);

        return aiResponse;
    }
}