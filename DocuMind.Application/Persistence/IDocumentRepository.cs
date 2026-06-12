using DocuMind.Domain;
using Pgvector;

namespace DocuMind.Application.Persistence;

public interface IDocumentRepository
{
    Task SaveChunksAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentChunk>> SearchHybridAsync(Vector queryEmbedding, string queryText, int limit, CancellationToken cancellationToken = default);
}