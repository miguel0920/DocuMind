namespace DocuMind.Application.Common;

public interface IChatService
{
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default);
}