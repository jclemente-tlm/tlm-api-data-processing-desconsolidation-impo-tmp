using System.Collections.Generic; // <--- ESTO ES VITAL PARA EL DICTIONARY

namespace Talma.AiServices.Core.DTOs;

// 1. Los datos específicos de la guía (lo que ya tenías)
public record AwbDataDto(
    string? AwbLeft,   
    string? AwbRight,
    string? ShipperName,
    string? ConsigneeRuc,
    string? PiecesRcp,
    string? GrossWeight,
    string? Currency,
    string? AirportOfDeparture,
    string? AirportOfDestination,
    string? TotalPrepaid,
    string? TotalCollect,
    string? HandlingInformation,
    string? NatureQuantityGoods,
    string? To1,
    string? To2,
    string? To3,
    string? IndPeligro
);

// 2. La estructura de cada página individual
public record PageResultDto(
    int PageNumber,
    bool IsAwb,            // ¿Es una guía aérea?
    string DocumentType,   // "Air Waybill", "Contract", "Invoice", "Unknown"
    AwbDataDto? Data       // Si es guía, trae los datos. Si no, viene null.
);

// 3. El objeto principal que recibirá el JSON de Gemini
public record DocumentProcessResponseDto(
    int TotalPages,
    List<PageResultDto> Pages
);