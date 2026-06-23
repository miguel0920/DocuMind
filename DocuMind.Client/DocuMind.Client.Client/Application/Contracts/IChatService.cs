using DocuMind.Client.Client.Core.Dtos;

namespace DocuMind.Client.Client.Application.Contracts;

public interface IChatService
{
    Task<QueryResponse> AskQuestionAsync(QueryRequest request);
}