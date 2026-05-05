namespace Talma.AiServices.Core.DTOs;

public class ApiResponse<T>
{
    public string Status { get; set; } = "success";
    public T? Data { get; set; }
    public List<ErrorInfo> Errors { get; set; } = new();
    public MetaData Meta { get; set; } = new();

    // Método para respuestas exitosas
    public static ApiResponse<T> Success(T data, string traceId) =>
        new() { 
            Status = "success", 
            Data = data, 
            Meta = new MetaData { TraceId = traceId } 
        };

    // Método para respuestas exitosas con advertencias
    public static ApiResponse<T> SuccessWithWarnings(T data, List<ErrorInfo> errors, string traceId) =>
        new() {
            Status = "success",
            Data = data,
            Errors = errors,
            Meta = new MetaData { TraceId = traceId }
        };

    // Método para respuestas con error
    public static ApiResponse<T> Error(string code, string message, string traceId, string tipo = "usr") =>
        new() {
            Status = "error",
            Data = default,
            Errors = new List<ErrorInfo> {
                new ErrorInfo { Code = code, Message = message, Tipo = tipo }
            },
            Meta = new MetaData { TraceId = traceId }
        };
}

public class MetaData
{
    public string TraceId { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ErrorInfo
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string Tipo { get; set; } = default!;
}