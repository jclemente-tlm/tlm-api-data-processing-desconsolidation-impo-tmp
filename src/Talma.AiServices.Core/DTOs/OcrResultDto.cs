namespace Talma.AiServices.Core.DTOs;

/// <summary>
/// Objeto ligero para transportar la respuesta del OCR entre capas.
/// Usamos 'record' de .NET 10 por ser inmutable y ligero.
/// </summary>
public record OcrResultDto(
    string FileName,
    string RawJson,
    DateTime ProcessedAt,
    bool IsSuccess
);