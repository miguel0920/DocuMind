using Microsoft.AspNetCore.Components.Forms;

namespace DocuMind.Client.Client.Application.Contracts;

public interface IDocumentService
{
    Task<bool> SendTextAsync(string text);
    Task<bool> UploadPdfsAsync(IReadOnlyList<IBrowserFile> files);
}