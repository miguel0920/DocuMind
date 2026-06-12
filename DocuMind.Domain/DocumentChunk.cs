using NpgsqlTypes;
using Pgvector;

namespace DocuMind.Domain
{
    public class DocumentChunk
    {
        public Guid Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }

        public Vector? Embedding { get; set; }

        public NpgsqlTsVector? SearchVector { get; set; }

        public DocumentChunk()
        {
            Id = Guid.NewGuid();
        }
    }
}