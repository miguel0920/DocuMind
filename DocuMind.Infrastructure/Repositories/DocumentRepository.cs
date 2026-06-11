using DocuMind.Application.Persistence;
using DocuMind.Domain;
using DocuMind.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Collections.Immutable;

namespace DocuMind.Infrastructure.Repositories;

public class DocumentRepository(DocuMindDbContext context) : IDocumentRepository
{
    private readonly DocuMindDbContext _context = context;

    public async Task SaveChunksAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default)
    {
        await _context.DocumentChunks.AddRangeAsync(chunks, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentChunk>> SearchSimilarChunksAsync(Vector queryEmbedding, int limit, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentChunks.AsNoTracking()
            // Buscamos los fragmentos ordenándolos por la menor distancia de coseno 
            // con respecto al vector de la pregunta del usuario.
            .OrderBy(c => c.Embedding!.CosineDistance(queryEmbedding))
            .Take(limit) // Tomamos solo los 'Top K' fragmentos más relevantes
            .ToListAsync(cancellationToken);
    }
}