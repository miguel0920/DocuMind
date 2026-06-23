using DocuMind.Client.Client.Application.Contracts;
using DocuMind.Client.Client.Core.Dtos;

namespace DocuMind.Client.Client.Infrastructure.ApiClients
{
    public class ChatApiClient : IChatService
    {
        public Task<QueryResponse> AskQuestionAsync(QueryRequest request)
        {
            throw new NotImplementedException();
        }
    }
}