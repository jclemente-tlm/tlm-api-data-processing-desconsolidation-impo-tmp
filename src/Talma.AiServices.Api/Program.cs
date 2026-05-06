using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Talma.AiServices.Core.Interfaces;
using Talma.AiServices.Core.Services;
using Talma.AiServices.Core.Validators;
using Talma.AiServices.Data.Repositories;
using Talma.AiServices.Infrastructure.Adapters;
using Talma.AiServices.Core.DTOs; // Aseguramos el acceso a ApiResponse
using Microsoft.AspNetCore.Mvc; // Para ApiBehaviorOptions


var builder = WebApplication.CreateBuilder(args);

// ===========================================================================

// 1. Configuración de Controladores y JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 🚀 CAMBIO SOLICITADO: Estandarización de errores de validación (Word/Excel/PDF Inválido)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorMessages = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        // Envolvemos el error en tu clase ApiResponse
        var response = ApiResponse<object>.Error(
            "VALIDATION_ERROR", 
            string.Join(" | ", errorMessages), 
            context.HttpContext.TraceIdentifier
        );

        return new BadRequestObjectResult(response);
    };
});

// 2. Validación Automática (FluentValidation)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateExtractionValidator>();

// 3. Inyección de Dependencias
builder.Services.AddScoped<IOcrRepository, OcrRepository>();
builder.Services.AddScoped<IGeminiAdapter, GeminiAdapter>();
builder.Services.AddScoped<OcrProcessorService>();

// 4. Hangfire con PostgreSQL 
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("PostgreSqlConnection")), 
    new PostgreSqlStorageOptions 
    {
        PrepareSchemaIfNecessary = true,
        SchemaName = "hangfire"
    }));

builder.Services.AddHangfireServer();

// 6. Health Checks
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Talma AI Services API",
        Version = "v1",
        Description = "Servicio OCR avanzado para procesamiento de guías aéreas."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// 6. Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Talma DESCO-IMPO v1");
    c.RoutePrefix = "swagger"; 
});

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new MyDashboardAuthorizationFilter() }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

Console.WriteLine("\n=================================================");
Console.WriteLine("MOTOR OCR TALMA - LISTO PARA OPERAR");
Console.WriteLine("=================================================\n");

app.Run();

public class MyDashboardAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
{
    public bool Authorize(Hangfire.Dashboard.DashboardContext context) => true;
}