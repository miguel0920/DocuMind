using DocuMind.Application.Persistence;
using DocuMind.Domain;
using DocuMind.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace DocuMind.Infrastructure.Repositories;

public class DocumentRepository(DocuMindDbContext context) : IDocumentRepository
{
    private readonly DocuMindDbContext _context = context;

    public async Task SaveChunksAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default)
    {
        await _context.DocumentChunks.AddRangeAsync(chunks, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentChunk>> SearchHybridAsync(Vector queryEmbedding, string queryText, int limit, CancellationToken cancellationToken = default)
    {
        // 1. Búsqueda Vectorial (Semántica - Entiende el contexto)
        // Extraemos el doble del límite solicitado para tener un buen margen de fusión
        var vectorResults = await _context.DocumentChunks
            .OrderBy(c => c.Embedding!.CosineDistance(queryEmbedding))
            .Take(limit * 2)
            .ToListAsync(cancellationToken);

        // 2. Búsqueda Full-Text Search (Palabras clave exactas)
        // Usamos PlainToTsQuery para que Postgres convierta la pregunta del usuario en formato de búsqueda seguro
        var ftsResults = await _context.DocumentChunks
            .Where(c => c.SearchVector!.Matches(EF.Functions.PlainToTsQuery("spanish", queryText)))
            .Take(limit * 2)
            .ToListAsync(cancellationToken);

        // 3. Algoritmo RRF (Reciprocal Rank Fusion)
        var rrfScores = new Dictionary<Guid, double>();
        const int k = 60; // 'k' es una constante estándar en la industria para estabilizar los puntajes

        // Punteamos los resultados vectoriales
        for (int i = 0; i < vectorResults.Count; i++)
        {
            var chunk = vectorResults[i];
            rrfScores[chunk.Id] = 1.0 / (k + i + 1); // A mejor posición, más puntos
        }

        // Punteamos los resultados de texto exacto
        for (int i = 0; i < ftsResults.Count; i++)
        {
            var chunk = ftsResults[i];
            // Si ya tenía puntos de la búsqueda vectorial, se los sumamos
            double currentScore = rrfScores.GetValueOrDefault(chunk.Id, 0);
            rrfScores[chunk.Id] = currentScore + (1.0 / (k + i + 1));
        }

        // 4. Consolidar, ordenar por mayor puntaje y devolver los ganadores definitivos
        var allChunks = vectorResults.Concat(ftsResults).DistinctBy(c => c.Id).ToDictionary(c => c.Id);

        var finalResults = rrfScores
            .OrderByDescending(kvp => kvp.Value) // Ordenamos de mayor a menor puntaje RRF
            .Take(limit) // Tomamos solo el límite que pidió el usuario (Top K)
            .Select(kvp => allChunks[kvp.Key])
            .ToList();

        return finalResults;
    }
}