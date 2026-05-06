using Talma.AiServices.Core.Interfaces;
using Talma.AiServices.Core.DTOs;
using System.Linq;
using System.IO;

namespace Talma.AiServices.Core.Services;

public class OcrProcessorService(IOcrRepository repository, IGeminiAdapter geminiAdapter) 
{
    public async Task ProcesarDocumentoAsync(int idSolicitud)
    {
        try 
        {
            await repository.ActualizarEstadoAsync(idSolicitud, "Lectura");
            var fileBytes = await repository.ObtenerArchivoPorIdAsync(idSolicitud);
            
            var solicitudInfo = await repository.ObtenerResultadoCompletoAsync(idSolicitud);
            string nombreArchivo = solicitudInfo?.NombreArchivo ?? "documento.pdf";
            string mimeType = GetMimeType(nombreArchivo);
            
            var resultado = await geminiAdapter.ProcessDocumentAsync(fileBytes, mimeType);
            
            if (resultado != null)
            {
                int totalHojas = resultado.TotalPages;
                int hojasValidas = resultado.Pages.Count(p => p.IsAwb);

                // ✅ SE MANTIENE: Actualiza la cabecera (Esto llena la columna total_paginas en ocr_solicitud)
                await repository.ActualizarContadoresPaginasAsync(idSolicitud, totalHojas, hojasValidas);

                // 🗑️ ELIMINADO: Ya no guardamos el ID 5 (Total Paginas) como un campo de página repetitivo
                // await repository.GuardarResultadoOcrAsync(idSolicitud, 1, 5, totalHojas.ToString());

                foreach (var pagina in resultado.Pages.Where(p => p.IsAwb))
                {
                    // 🗑️ ELIMINADO: Ya no guardamos el ID 6 (Tipo Documento) en cada página
                    // await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 6, pagina.DocumentType);

                    if (pagina.Data != null)
                    {
                        // Solo guardamos los campos de negocio (Los 13 campos clave)
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 1,  pagina.Data.AwbLeft ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 2,  pagina.Data.AwbRight ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 3,  pagina.Data.ShipperName ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 4,  pagina.Data.ConsigneeRuc ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 15, pagina.Data.AirportOfDeparture ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 17, pagina.Data.AirportOfDestination ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 21, pagina.Data.PiecesRcp ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 22, pagina.Data.GrossWeight ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 23, pagina.Data.Currency ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 38, pagina.Data.TotalPrepaid ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 39, pagina.Data.TotalCollect ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 34, pagina.Data.HandlingInformation ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 20, pagina.Data.NatureQuantityGoods ?? "");
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 35, pagina.Data.To1 ?? ""); // TO1
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 36, pagina.Data.To2 ?? ""); // TO2
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 37, pagina.Data.To3 ?? ""); // TO3
                        await repository.GuardarResultadoOcrAsync(idSolicitud, pagina.PageNumber, 27, pagina.Data.IndPeligro ?? ""); // IND_PELIGRO
                    }
                }
            }

            await repository.ActualizarEstadoAsync(idSolicitud, "Finalizado");
            await repository.EliminarArchivoBinarioAsync(idSolicitud);
        }
        catch (Exception)
        {
            await repository.ActualizarEstadoAsync(idSolicitud, "Error");
            throw;
        }
    }

    private string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/pdf" 
        };
    }
}