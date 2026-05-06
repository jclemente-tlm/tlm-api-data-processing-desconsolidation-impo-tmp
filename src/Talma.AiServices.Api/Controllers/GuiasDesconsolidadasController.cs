using Microsoft.AspNetCore.Mvc;
using Talma.AiServices.Core.Interfaces;
using Talma.AiServices.Core.DTOs;
using Talma.AiServices.Core.Services;
using Hangfire;

namespace Talma.AiServices.Api.Controllers;


[ApiController]
[Produces("application/json")]
public abstract class GuiasDesconsolidadasControllerBase(IOcrRepository repository) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OcrResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<ApiResponse<OcrResponseDto>>> Create([FromBody] OcrRequestDto request)
    {
        try
        {
            // 1. Decodificación de Base64
            byte[] fileBytes = Convert.FromBase64String(request.FileBase64);
            
            // 2. Extracción de Usuario desde la Metadata del DTO
            string usuario = request.Metadata.User; 

            // 3. Registro en Persistencia (Base de Datos PostgreSQL)
            // Se pasan los 5 parámetros requeridos por la arquitectura actual
            int idSolicitud = await repository.RegistrarSolicitudOcrAsync(
                fileBytes, 
                request.FileName, 
                usuario,
                request.MailId,     // Enviamos el MailId del JSON
                request.GuiaMaster  // Enviamos la GuiaMaster del JSON
            );

            // 4. Procesamiento Asíncrono con Hangfire
            // Esto libera el hilo de ejecución de inmediato
            BackgroundJob.Enqueue<OcrProcessorService>(x => x.ProcesarDocumentoAsync(idSolicitud));

            // 5. Construcción de la respuesta exitosa
            var resultData = new OcrResponseDto(
                IdSolicitud: idSolicitud,
                Estado: "R", // R de Registrado
                Mensaje: "Documento recibido correctamente. Procesamiento en curso."
            );

            // 6. Envolver en ApiResponse estándar de Talma
            var response = ApiResponse<OcrResponseDto>.Success(
                resultData, 
                HttpContext.TraceIdentifier
            );

            return Created("", response);
        }
        catch (Exception ex)
        {
            // Manejo de errores global del controlador
            return StatusCode(500, ApiResponse<object>.Error(
                "INTERNAL_ERROR", 
                ex.Message, 
                HttpContext.TraceIdentifier));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ExtractionDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<ExtractionDetailDto>>> GetById(int id)
    {
        try
        {
            // 1. Consulta al repositorio para obtener resultados del OCR
            var result = await repository.ObtenerResultadoCompletoAsync(id);
            
            // 2. Validación de existencia
            if (result == null)
            {
                return NotFound(ApiResponse<object>.Error(
                    "NOT_FOUND", 
                    $"No se encontró la solicitud con ID {id}", 
                    HttpContext.TraceIdentifier));
            }

            // 3. Retornar éxito con el wrapper estándar
            return Ok(ApiResponse<ExtractionDetailDto>.Success(
                result, 
                HttpContext.TraceIdentifier));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error(
                "INTERNAL_ERROR", 
                ex.Message, 
                HttpContext.TraceIdentifier));
        }
    }
}


[Route("api/v1/extracciones/guias-desconsolidadas")]
public class  GuiaDesconsolidadasController(IOcrRepository repository) 
    : ExtractionsControllerBase(repository);

