using DocuMind.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocuMind.Application.Documents.Commands
{
    public record IngestPdfCommand(Stream PdfStream, string FileName) : IRequest<bool>;

    public class IngestPdfCommandHandler(IPdfExtractorService pdfExtractor, IMediator mediator) : IRequestHandler<IngestPdfCommand, bool>
    {
        private readonly IPdfExtractorService _pdfExtractor = pdfExtractor;
        private readonly IMediator _mediator = mediator;

        public async Task<bool> Handle(IngestPdfCommand request, CancellationToken cancellationToken)
        {
            string extractedText = _pdfExtractor.ExtractTextFromPdf(request.PdfStream);

            if (string.IsNullOrWhiteSpace(extractedText))
                return false;

            var textIngestCommand = new IngestDocumentCommand(request.FileName, extractedText);

            return await _mediator.Send(textIngestCommand, cancellationToken);
        }
    }
}