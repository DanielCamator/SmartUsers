using ALService.Application.Interfaces;
using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiAgentController : ControllerBase
{
    private readonly IAiAssistantService _aiAssistantService;
    private readonly ILogger<AiAgentController> _logger;

    public AiAgentController(
        IAiAssistantService aiAssistantService,
        ILogger<AiAgentController> logger)
    {
        _aiAssistantService = aiAssistantService;
        _logger = logger;
    }

    /// <summary>
    /// Consulta al Agente de IA de SmartUsers usando RAG.
    /// Incluye evaluación de latencia y costo de tokens.
    /// </summary>
    [HttpPost("ask")]
    [ProducesResponseType(typeof(AiResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AskQuestion([FromBody] AiRequestDto request, CancellationToken ct)
    {
        try
        {
            var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
                ? Guid.NewGuid().ToString()
                : request.SessionId;

            _logger.LogInformation("Procesando pregunta de IA. SessionId: {SessionId}", sessionId);

            var aiResponse = await _aiAssistantService.AskQuestionAsync(request.Question, sessionId, ct);

            return Ok(new
            {
                SessionId = sessionId,
                Data = aiResponse
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la solicitud de IA.");
            return StatusCode(500, new { message = "Ocurrió un error al procesar la respuesta del agente.", details = ex.Message });
        }
    }
}