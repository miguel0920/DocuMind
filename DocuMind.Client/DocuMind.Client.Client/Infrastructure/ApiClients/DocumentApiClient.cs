using DocuMind.Client.Client.Application.Contracts;
using Microsoft.AspNetCore.Components.Forms;

namespace DocuMind.Client.Client.Infrastructure.ApiClients
{
    public class DocumentApiClient : IDocumentService
    {
        public Task<bool> SendTextAsync(string text)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadPdfsAsync(IReadOnlyList<IBrowserFile> files)
        {
            throw new NotImplementedException();
        }
    }
}