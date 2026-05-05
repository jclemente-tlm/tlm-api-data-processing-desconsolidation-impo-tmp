using Microsoft.AspNetCore.Mvc;
using Talma.AiServices.Core.Interfaces;
using Talma.AiServices.Core.DTOs;
using Talma.AiServices.Core.Services;
using Hangfire;
using System.Data.Common;

namespace Talma.AiServices.Api.Controllers;


[ApiController]
[Produces("application/json")]
public abstract class ExtractionsControllerBase(IOcrRepository repository) : ControllerBase
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
        catch (DbException)
        {
            return StatusCode(503, ApiResponse<object>.Error(
                "DB_UNAVAILABLE",
                "La base de datos no está disponible. Por favor intente más tarde.",
                HttpContext.TraceIdentifier,
                tipo: "tecnico"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error(
                "INTERNAL_ERROR",
                ex.Message,
                HttpContext.TraceIdentifier,
                tipo: "tecnico"));
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

            // 3. Si el procesamiento finalizó y no hay hojas válidas, retornar éxito con advertencia informativa
            if (result.Estado == "F" && result.CantidadHojasValidas == 0)
            {
                var warnings = new List<ErrorInfo>
                {
                    new ErrorInfo { Code = "NO_AWB_FOUND", Message = "El documento no contiene guías aéreas válidas. Solo se procesan Air Waybills.", Tipo = "usr" }
                };
                return Ok(ApiResponse<ExtractionDetailDto>.SuccessWithWarnings(
                    result,
                    warnings,
                    HttpContext.TraceIdentifier));
            }

            // 4. Retornar éxito con el wrapper estándar
            return Ok(ApiResponse<ExtractionDetailDto>.Success(
                result,
                HttpContext.TraceIdentifier));
        }
        catch (DbException)
        {
            return StatusCode(503, ApiResponse<object>.Error(
                "DB_UNAVAILABLE",
                "La base de datos no está disponible. Por favor intente más tarde.",
                HttpContext.TraceIdentifier,
                tipo: "tecnico"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Error(
                "INTERNAL_ERROR",
                ex.Message,
                HttpContext.TraceIdentifier,
                tipo: "tecnico"));
        }
    }
}

// ==========================================================
// CONTROLADOR 2: PRUEBAS INTERNAS
// ==========================================================
[Route("api/v1/solicitudes-extraccion-desco-impo")]
public class ExtractionsController(IOcrRepository repository) 
    : ExtractionsControllerBase(repository);

// ==========================================================
// CONTROLADOR 1: PRODUCCIÓN
// ==========================================================
[Route("api/v1/extraccion-desco-impo")]
public class InternalTestExtractionsController(IOcrRepository repository) 
    : ExtractionsControllerBase(repository);