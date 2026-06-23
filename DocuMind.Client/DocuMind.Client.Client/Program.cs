using DocuMind.Client.Client.Application.Contracts;
using DocuMind.Client.Client.Infrastructure.ApiClients;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7123/") // Reemplaza con el puerto real de tu WebAPI
});

builder.Services.AddScoped<IDocumentService, DocumentApiClient>();
builder.Services.AddScoped<IChatService, ChatApiClient>();

await builder.Build().RunAsync();
