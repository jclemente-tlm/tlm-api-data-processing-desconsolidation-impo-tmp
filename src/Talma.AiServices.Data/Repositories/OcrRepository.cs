using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Talma.AiServices.Core.Interfaces;
using Talma.AiServices.Core.DTOs;

namespace Talma.AiServices.Data.Repositories;

public class OcrRepository(IConfiguration config) : IOcrRepository
{
    private readonly string _connectionString = config.GetConnectionString("PostgreSqlConnection")!;

    // 1. Registro inicial (Tu código actual)
    public async Task<int> RegistrarSolicitudOcrAsync(byte[] pdfBytes, string fileName, string usuario, string? mailId, string? guiaMaster)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var parameters = new {
            p_guia_master = guiaMaster,
            p_mail_id = mailId ?? "mail-" + Guid.NewGuid().ToString()[..8],
            p_nombre_archivo = fileName,
            p_ruta_archivo = "DB_STORAGE",
            p_archivo_binario = pdfBytes,
            p_usuario_registro = usuario
        };

        return await connection.ExecuteScalarAsync<int>(
            "SELECT sp_registrar_ocr(@p_guia_master, @p_mail_id, @p_nombre_archivo, @p_ruta_archivo, @p_archivo_binario, @p_usuario_registro)",
            parameters);
    }

    // 2. Obtener archivo para Gemini
    public async Task<byte[]> ObtenerArchivoPorIdAsync(int idSolicitud)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<byte[]>(
            "SELECT archivo_binario FROM ocr_solicitud WHERE id_solicitud = @idSolicitud", 
            new { idSolicitud }) ?? [];
    }

    // 3. Guardar resultado usando el Stored Procedure que creamos
    public async Task GuardarResultadoOcrAsync(int idSolicitud, int numPagina, int idCampo, string valor)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "CALL sp_guardar_resultado_ocr(@p_id_solicitud, @p_num_pagina, @p_id_campo, @p_valor)", 
            new { 
                p_id_solicitud = idSolicitud, 
                p_num_pagina = numPagina, 
                p_id_campo = idCampo, 
                p_valor = valor 
            });
    }

    // 4. Actualizar el estado del proceso
    public async Task ActualizarEstadoAsync(int idSolicitud, string estado)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE ocr_solicitud SET estado_actual = @estado WHERE id_solicitud = @idSolicitud", 
            new { idSolicitud, estado });
    }

    // 5. Vaciamos el campo binario para ahorrar almacenamiento
    public async Task EliminarArchivoBinarioAsync(int idSolicitud)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE ocr_solicitud SET archivo_binario = NULL WHERE id_solicitud = @idSolicitud", 
            new { idSolicitud });
    }

    //  6. Actualizamos hojas validas para el proceso
    public async Task ActualizarHojasValidasAsync(int idSolicitud, int cantidadValidas)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        const string sql = "UPDATE ocr_solicitud SET cantidad_hojas_validas = @cantidadValidas WHERE id_solicitud = @idSolicitud";
        
        await connection.ExecuteAsync(sql, new { idSolicitud, cantidadValidas });
    }

    // 7. Actualizar contadores de páginas
    public async Task ActualizarContadoresPaginasAsync(int idSolicitud, int totalHojas, int hojasValidas)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        const string sql = @"
            UPDATE ocr_solicitud 
            SET total_hojas = @totalHojas, 
                cantidad_hojas_validas = @hojasValidas 
            WHERE id_solicitud = @idSolicitud";
        
        await connection.ExecuteAsync(sql, new { idSolicitud, totalHojas, hojasValidas });
    }

    // 8. Obtener resultado completo
    public async Task<ExtractionDetailDto?> ObtenerResultadoCompletoAsync(int idSolicitud)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        
        // SQL alineado exactamente con tus tablas public.ocr_solicitud y public.resultado_lectura
        const string sql = @"
            SELECT 
                s.id_solicitud, 
                s.estado_actual, 
                s.nombre_archivo, 
                s.cantidad_hojas_validas, 
                s.total_hojas, 
                s.fecha_registro,
                r.num_pagina, 
                r.id_campo, 
                m.nombre_campo, 
                r.valor_obtenido as valor
            FROM public.ocr_solicitud s
            LEFT JOIN public.resultado_lectura r ON s.id_solicitud = r.id_solicitud
            LEFT JOIN public.maestro_campo m ON r.id_campo = m.id_campo
            WHERE s.id_solicitud = @idSolicitud
            ORDER BY r.num_pagina, r.id_campo";

        var rows = await connection.QueryAsync<dynamic>(sql, new { idSolicitud });

        if (!rows.Any()) return null;

        ExtractionDetailDto? detail = null;
        var pagesDict = new Dictionary<int, ExtractionPageDto>();

        foreach (var row in rows)
        {
            if (detail == null)
            {
                // 1. Mapeo de estado a código corto (R, L, E, F)
                string estadoDb = (string)row.estado_actual;
                string estadoCorto = estadoDb switch
                {
                    "Registrado" => "R",
                    "Lectura"    => "L",
                    "Error"      => "E",
                    "Finalizado" => "F",
                    _            => estadoDb
                };

                // 2. Creación del DTO principal con la nueva columna total_hojas
                detail = new ExtractionDetailDto(
                    (int)row.id_solicitud,
                    estadoCorto,
                    (string)row.nombre_archivo,
                    (int)(row.cantidad_hojas_validas ?? 0),
                    (int)(row.total_hojas ?? 0), // 🚀 Mapeo exitoso
                    (DateTime)row.fecha_registro,
                    new List<ExtractionPageDto>() 
                );
            }

            // ✅ REGLA DE ORO: Solo llenamos datos si el estado es 'F' (Finalizado)
            if (detail.Estado == "F" && row.num_pagina != null)
            {
                int idCampo = (int)row.id_campo;

                // 🚀 LIMPIEZA: No enviamos los IDs 5 (Total Hojas) ni 6 (Tipo Doc) en el array de campos
                if (idCampo == 5 || idCampo == 6) continue;

                int pageNum = (int)row.num_pagina;
                if (!pagesDict.TryGetValue(pageNum, out var pageDto))
                {
                    pageDto = new ExtractionPageDto(pageNum, new List<ExtractionFieldDto>());
                    pagesDict.Add(pageNum, pageDto);
                    detail.Paginas.Add(pageDto);
                }

                pageDto.Fields.Add(new ExtractionFieldDto(
                    idCampo,
                    (string)row.nombre_campo,
                    (string)(row.valor ?? "")
                ));
            }
        }

        return detail;
    }


}