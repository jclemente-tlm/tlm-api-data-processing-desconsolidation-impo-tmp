using Talma.AiServices.Core.DTOs;

namespace Talma.AiServices.Core.Interfaces;

public interface IGeminiAdapter
{
    // Cambiamos el tipo de retorno al nuevo contenedor de múltiples páginas
    Task<DocumentProcessResponseDto?> ProcessDocumentAsync(byte[] fileBytes, string mimeType);
}