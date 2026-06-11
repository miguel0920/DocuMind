using DocuMind.Application.Documents.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocuMind.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("ingest")]
    public async Task<IActionResult> IngestDocument([FromBody] IngestRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("El contenido del documento no puede estar vacío.");

        // Creamos el comando CQRS
        var command = new IngestDocumentCommand(request.DocumentName, request.Content);

        // Enviamos el comando a través del bus de MediatR
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