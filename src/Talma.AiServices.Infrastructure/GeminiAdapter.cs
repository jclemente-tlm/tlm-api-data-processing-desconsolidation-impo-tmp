using Google.Cloud.AIPlatform.V1;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Talma.AiServices.Core.Interfaces;
using Talma.AiServices.Core.DTOs;

namespace Talma.AiServices.Infrastructure.Adapters;

public class GeminiAdapter : IGeminiAdapter
{
    private readonly string _projectId;
    private readonly string _location;
    private readonly string _modelId = "gemini-2.5-flash"; 
    private readonly PredictionServiceClient _predictionServiceClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public GeminiAdapter(IConfiguration config)
    {
        _projectId = config["GoogleCloud:ProjectId"] ?? throw new ArgumentNullException("GCP ProjectId is missing");
        _location = config["GoogleCloud:Location"] ?? "us-central1";
        _predictionServiceClient = PredictionServiceClient.Create();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<DocumentProcessResponseDto?> ProcessDocumentAsync(byte[] fileBytes, string mimeType)
    {
        var endpoint = EndpointName.FromProjectLocationPublisherModel(_projectId, _location, "google", _modelId);

        // Prompt optimizado para filtrado y extracción multimodal
        string systemPrompt = @"
ROL: Experto en logística de Talma y especialista en reconocimiento de documentos aeronáuticos.
TAREA: Analiza CADA PÁGINA de forma independiente y clasifícala antes de extraer los datos.

REGLAS CRÍTICAS DE CLASIFICACIÓN:
1. 'IsAwb' (Booleano): 
   - Debe ser TRUE solo si la página es una 'Air Waybill' (Guía Aérea) con datos de transporte.
   - Debe ser FALSE si la página es un Contrato, Términos y Condiciones, Manifiestos o cualquier otro documento.
2. 'DocumentType' (String): 
   - Debe ser exacto: 'Air Waybill', 'Contract', 'Terms and Conditions' o 'Other'.

REGLAS DE EXTRACCIÓN (Solo si IsAwb es TRUE):
1. 'AwbLeft' y 'AwbRight': Números de guía en las esquinas superiores AwbLeft le pertenece a la esquina izquierda y AwbRight a la derecha.
2. 'ShipperName': Nombre del remitente.
3. 'ConsigneeRuc': RUC de 11 dígitos del consignatario.
4. 'PiecesRcp': Cantidad debajo de 'No. of Pieces RCP'.
5. 'GrossWeight': Valor debajo de 'Gross Weight'.
6. 'Currency': Código de moneda (USD, EUR, SOL).
7. 'AirportOfDeparture': Nombre o código IATA de origen.
8. 'AirportOfDestination': Nombre o código IATA de destino.
9. 'TotalPrepaid': En la sección de cargos del AWB existe una fila o recuadro con el label 'Total Prepaid'. El valor que necesitas es el MONTO TOTAL FINAL de esa fila, que aparece en la celda más a la derecha (columna 'Total') de la fila 'Prepaid', o bien en un recuadro resumen al pie con el label 'Total Prepaid'. NO tomes valores parciales ni subtotales de otras columnas intermedias de esa misma fila. Si la celda está vacía o en blanco, retornar cadena vacía (es normal que solo TotalCollect tenga valor).
10. 'TotalCollect': En la sección de cargos del AWB existe una fila o recuadro con el label 'Total Collect'. El valor que necesitas es el MONTO TOTAL FINAL de esa fila, que aparece en la celda más a la derecha (columna 'Total') de la fila 'Collect', o bien en un recuadro resumen al pie con el label 'Total Collect'. NO tomes valores parciales ni subtotales de otras columnas intermedias de esa misma fila. Si la celda está vacía o en blanco, retornar cadena vacía (es normal que solo TotalPrepaid tenga valor).
11. 'HandlingInformation': Texto completo del recuadro 'Handling Information'.
12. 'NatureQuantityGoods': Descripción completa de la columna central (incluye dimensiones).
13. 'To1': Primer aeropuerto de transferencia (primera caja 'To' en el encabezado de routing).
14. 'To2': Segundo aeropuerto de transferencia (segunda caja 'To' en el encabezado de routing).
15. 'To3': Tercer aeropuerto de transferencia (tercera caja 'To' en el encabezado de routing). Si no existe, dejar vacío.
16. 'IndPeligro': Si el recuadro 'Handling Information' contiene un código de mercancía peligrosa con formato UN seguido de exactamente 4 dígitos (ej: UN1263, UN3077), extraer ese código. Si no existe ningún código UN, retornar cadena vacía.

REGLA DE ORO PARA AEROPUERTOS:
- Prioriza siempre el código IATA de 3 letras (ej. LIM, MIA).

ESTRUCTURA JSON OBLIGATORIA:
{
    ""TotalPages"": integer,
    ""Pages"": [
        {
            ""PageNumber"": integer,
            ""IsAwb"": boolean,
            ""DocumentType"": ""string"",
            ""Data"": {
                ""AwbLeft"": ""string"", ""AwbRight"": ""string"", ""ShipperName"": ""string"", 
                ""ConsigneeRuc"": ""string"", ""PiecesRcp"": ""string"", ""GrossWeight"": ""string"", 
                ""Currency"": ""string"", ""AirportOfDeparture"": ""string"", ""AirportOfDestination"": ""string"", 
                ""TotalPrepaid"": ""string"", ""TotalCollect"": ""string"", ""HandlingInformation"": ""string"",
                ""NatureQuantityGoods"": ""string"",
                ""To1"": ""string"", ""To2"": ""string"", ""To3"": ""string"",
                ""IndPeligro"": ""string""
            }
        }
    ]
}";

        var generateContentRequest = new GenerateContentRequest
        {
            Model = endpoint.ToString(),
            SystemInstruction = new Content { Parts = { new Part { Text = systemPrompt } } },
            Contents = {
                new Content {
                    Role = "user",
                    Parts = {
                        new Part { Text = "Analiza el documento, clasifica cada página y extrae datos solo si corresponde a una Air Waybill." },
                        new Part { InlineData = new Blob { MimeType = mimeType, Data = ByteString.CopyFrom(fileBytes) } }
                    }
                }
            },
            GenerationConfig = new GenerationConfig { ResponseMimeType = "application/json", Temperature = 0.0f }
        };

        var response = await _predictionServiceClient.GenerateContentAsync(generateContentRequest);
        string rawJson = response.Candidates[0].Content.Parts[0].Text;

        return JsonSerializer.Deserialize<DocumentProcessResponseDto>(rawJson, _jsonOptions);
    }
}