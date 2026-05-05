using Talma.AiServices.Core.DTOs;

namespace Talma.AiServices.Core.Interfaces;

public interface IOcrRepository
{
    Task<int> RegistrarSolicitudOcrAsync(byte[] pdfBytes, string fileName, string usuario, string? mailId, string? guiaMaster);    Task<byte[]> ObtenerArchivoPorIdAsync(int idSolicitud);
    Task GuardarResultadoOcrAsync(int idSolicitud, int numPagina, int idCampo, string valor);
    Task ActualizarEstadoAsync(int idSolicitud, string estado);
    Task EliminarArchivoBinarioAsync(int idSolicitud);
    Task ActualizarHojasValidasAsync(int idSolicitud, int cantidadValidas);
    Task ActualizarContadoresPaginasAsync(int idSolicitud, int totalHojas, int hojasValidas);   
    Task<ExtractionDetailDto?> ObtenerResultadoCompletoAsync(int idSolicitud);
}