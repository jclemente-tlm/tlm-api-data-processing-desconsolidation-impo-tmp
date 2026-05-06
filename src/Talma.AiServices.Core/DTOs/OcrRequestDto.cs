namespace Talma.AiServices.Core.DTOs;

public record OcrRequestDto
{
    public string? MailId { get; init; }
    public string? GuiaMaster { get; init; }

    /// <summary>Nombre del archivo con extensión</summary>
    /// <example>guia_aerea_001.pdf</example>
    public required string FileName { get; init; }
    /// <summary>Contenido del archivo en formato Base64 (sin saltos de línea)</summary>
    public required string FileBase64 { get; init; }

    public RequestMetadataDto Metadata { get; init; } = new();
}

public record RequestMetadataDto
{
    public string User { get; init; } = "system";
    public string Source { get; init; } = "unknown";
}