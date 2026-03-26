using Microsoft.AspNetCore.Mvc;
using ALService.Application.Interfaces;

namespace ALService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngestionController : ControllerBase
{
    private readonly IIngestionService _ingestionService;
    private readonly ILogger<IngestionController> _logger;

    public IngestionController(IIngestionService ingestionService, ILogger<IngestionController> logger)
    {
        _ingestionService = ingestionService;
        _logger = logger;
    }

    /// <summary>
    /// Lee el archivo JSON local y sube los vectores a Pinecone.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UploadDataToPinecone()
    {
        try
        {
            _logger.LogInformation("Iniciando ingesta de datos a Pinecone...");

            await _ingestionService.CreateIndexAndUploadAsync();

            _logger.LogInformation("Ingesta completada con éxito.");
            return Ok(new { message = "Datos de SmartUsers cargados en Pinecone exitosamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la ingesta de datos.");
            return StatusCode(500, new { message = "Error al procesar la ingesta.", details = ex.Message });
        }
    }
}