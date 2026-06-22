using DocuMind.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;
using UglyToad.PdfPig;

namespace DocuMind.Infrastructure.Services
{
    public class PdfExtractorService : IPdfExtractorService
    {
        public string ExtractTextFromPdf(Stream pdfStream)
        {
            if (pdfStream == null || pdfStream.Length == 0)
                return string.Empty;

            var textBuilder = new StringBuilder();

            using (PdfDocument document = PdfDocument.Open(pdfStream))
            {
                foreach (var page in document.GetPages())
                {
                    // Extrae las palabras de cada página
                    string pageText = page.Text;
                    textBuilder.AppendLine(pageText);
                }
            }

            return textBuilder.ToString();
        }
    }
}