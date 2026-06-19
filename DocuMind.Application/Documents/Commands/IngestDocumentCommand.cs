using DocuMind.Application.Common;
using DocuMind.Application.Persistence;
using DocuMind.Domain;
using MediatR;

namespace DocuMind.Application.Documents.Commands;

public record IngestDocumentCommand(string DocumentName, string Content) : IRequest<bool>;

public class IngestDocumentCommandHandler(ITextSplitterService splitterService,
                                          IEmbeddingService embeddingService,
                                          IDocumentRepository documentRepository) : IRequestHandler<IngestDocumentCommand, bool>
{
    private readonly ITextSplitterService _splitterService = splitterService;
    private readonly IEmbeddingService _embeddingService = embeddingService;
    private readonly IDocumentRepository _documentRepository = documentRepository;

    public async Task<bool> Handle(IngestDocumentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return false;

        // Configuramos tamaño de chunk (1000 caracteres) y solapamiento (200 caracteres)
        int chunkSize = 1000;
        int chunkOverlap = 200;

        // 1. Fragmentamos el texto plano
        var textChunks = _splitterService.SplitText(request.Content, chunkSize, chunkOverlap);
        var documentChunks = new List<DocumentChunk>();
        int index = 0;

        // 2. Procesamos cada fragmento para generar su "huella matemática" (Embedding)
        foreach (var text in textChunks)
        {
            float[] embedding = await _embeddingService.GenerateEmbeddingAsync(text, cancellationToken);

            var chunk = new DocumentChunk
            {
                DocumentName = request.DocumentName,
                Content = text,
                ChunkIndex = index++,
                Embedding = new Pgvector.Vector(embedding)
            };

            documentChunks.Add(chunk);
        }

        await _documentRepository.SaveChunksAsync(documentChunks, cancellationToken);

        return true;
    }
}