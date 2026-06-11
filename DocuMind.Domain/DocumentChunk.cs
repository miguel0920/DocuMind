using Pgvector;

namespace DocuMind.Domain
{
    public class DocumentChunk
    {
        public Guid Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }

        // Aquí se guardará el vector que nos devuelva Gemini (Array de floats)
        public Vector? Embedding { get; set; }

        public DocumentChunk()
        {
            Id = Guid.NewGuid();
        }
    }
}