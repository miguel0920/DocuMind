namespace DocuMind.Application.Common;

public interface IPdfExtractorService
{
    string ExtractTextFromPdf(Stream pdfStream);
}