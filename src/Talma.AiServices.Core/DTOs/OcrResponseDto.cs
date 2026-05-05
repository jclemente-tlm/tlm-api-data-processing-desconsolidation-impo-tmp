namespace Talma.AiServices.Core.DTOs;

/// <summary>Representa la respuesta inmediata tras registrar una solicitud de OCR</summary>
public record OcrResponseDto(
    /// <summary>Identificador único de la solicitud en base de datos</summary>
    /// <example>1524</example>
    int IdSolicitud, 
    
    /// <summary>Estado inicial del proceso (R: Recibido, P: Procesando)</summary>
    /// <example>R</example>
    string Estado, 
    
    /// <summary>Mensaje descriptivo para el usuario</summary>
    /// <example>Documento recibido correctamente. Procesamiento en curso.</example>
    string Mensaje
) {
    /// <summary>Fecha y hora UTC en la que se registró la solicitud</summary>
    public DateTime Fecha { get; init; } = DateTime.UtcNow;
}