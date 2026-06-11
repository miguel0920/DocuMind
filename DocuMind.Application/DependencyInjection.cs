using DocuMind.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DocuMind.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registra MediatR buscando todos los handlers en este proyecto de Application
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Registra nuestro servicio de Splitter
        services.AddScoped<ITextSplitterService, TextSplitterService>();

        return services;
    }
}