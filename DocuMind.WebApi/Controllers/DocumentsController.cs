using DocuMind.Application.Documents.Commands;
using DocuMind.Application.Documents.Queries;
using DocuMind.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocuMind.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("ask")]
    public async Task<IActionResult> AskDocument([FromBody] AskRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.QueryText))
            return BadRequest("La pregunta no puede estar vacía.");

        // Configuramos un límite de 4 fragmentos (Top 4) para darle buen contexto a la IA
        var query = new SearchVectorQuery(request.QueryText, Limit: 4);

        RagResponseDto response = await _mediator.Send(query, cancellationToken);

        return Ok(new { answer = response });
    }

    [HttpPost("ingest")]
    public async Task<IActionResult> IngestDocument([FromBody] IngestRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("El contenido del documento no puede estar vacío.");
        var command = new IngestDocumentCommand(request.DocumentName, request.Content);

        bool result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return StatusCode(500, "Hubo un error al procesar o vectorizar el documento.");

        return Ok(new { message = $"Documento '{request.DocumentName}' procesado, fragmentado y vectorizado con éxito." });
    }
}

// Clase de utilidad para recibir el JSON del cliente
public class IngestRequest
{
    public string DocumentName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class AskRequest
{
    public string QueryText { get; set; } = string.Empty;
}