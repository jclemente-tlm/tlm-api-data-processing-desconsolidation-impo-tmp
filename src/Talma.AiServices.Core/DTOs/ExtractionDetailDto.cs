namespace Talma.AiServices.Core.DTOs;

/// <summary>Representa un campo individual extraído de la base de datos</summary>
public record ExtractionFieldDto(int IdCampo, string NombreCampo, string Valor);

/// <summary>Agrupa los campos extraídos por cada página del documento</summary>
public record ExtractionPageDto(int PageNumber, List<ExtractionFieldDto> Fields);

/// <summary>DTO principal para la respuesta detallada del GET</summary>
public record ExtractionDetailDto(
    int IdSolicitud, 
    string Estado, 
    string NombreArchivo,
    int CantidadHojasValidas,
    int TotalHojas,
    DateTime FechaRegistro,
    List<ExtractionPageDto> Paginas
);