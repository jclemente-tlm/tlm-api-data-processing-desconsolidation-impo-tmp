using Moq;
using Talma.AiServices.Core.DTOs;
using Talma.AiServices.Core.Interfaces;
using Talma.AiServices.Core.Services;
using Xunit;

namespace Talma.AiServices.Core.Tests.Services;

public class OcrProcessorServiceTests
{
    private readonly Mock<IOcrRepository> _repo = new();
    private readonly Mock<IGeminiAdapter> _gemini = new();
    private readonly OcrProcessorService _sut;

    private static readonly byte[] FakeFileBytes = [0x25, 0x50, 0x44, 0x46];

    public OcrProcessorServiceTests()
    {
        _repo.Setup(r => r.ObtenerArchivoPorIdAsync(It.IsAny<int>()))
             .ReturnsAsync(FakeFileBytes);
        _repo.Setup(r => r.ActualizarEstadoAsync(It.IsAny<int>(), It.IsAny<string>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.ActualizarContadoresPaginasAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.GuardarResultadoOcrAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.EliminarArchivoBinarioAsync(It.IsAny<int>()))
             .Returns(Task.CompletedTask);

        _sut = new OcrProcessorService(_repo.Object, _gemini.Object);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static AwbDataDto BuildAwbData() => new(
        AwbLeft: "123", AwbRight: "456", ShipperName: "SHIPPER", ConsigneeRuc: "RUC123",
        PiecesRcp: "10", GrossWeight: "100.5", Currency: "USD",
        AirportOfDeparture: "BOG", AirportOfDestination: "GRU",
        TotalPrepaid: "50.00", TotalCollect: "0.00",
        HandlingInformation: "FRAGILE", NatureQuantityGoods: "ELECTRONICS",
        To1: "LIM", To2: "", To3: "", IndPeligro: "N"
    );

    private static DocumentProcessResponseDto BuildResponse(int totalPages, IEnumerable<PageResultDto> pages) =>
        new(totalPages, pages.ToList());

    // ── Estado y flujo cuando Gemini devuelve null ───────────────────────────

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenGeminiReturnsNull_ShouldFinalizarWithoutSavingPages()
    {
        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), "application/pdf"))
               .ReturnsAsync((DocumentProcessResponseDto?)null);

        await _sut.ProcesarDocumentoAsync(1);

        _repo.Verify(r => r.ActualizarContadoresPaginasAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _repo.Verify(r => r.GuardarResultadoOcrAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        _repo.Verify(r => r.ActualizarEstadoAsync(1, "Finalizado"), Times.Once);
        _repo.Verify(r => r.EliminarArchivoBinarioAsync(1), Times.Once);
    }

    // ── Estado inicial siempre se establece en "Lectura" ────────────────────

    [Fact]
    public async Task ProcesarDocumentoAsync_ShouldSetEstadoLecturaFirst()
    {
        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
               .ReturnsAsync((DocumentProcessResponseDto?)null);

        await _sut.ProcesarDocumentoAsync(1);

        _repo.Verify(r => r.ActualizarEstadoAsync(1, "Lectura"), Times.Once);
    }

    // ── Páginas AWB → guarda los 17 campos ──────────────────────────────────

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenResultHasOneAwbPage_ShouldGuardar17Fields()
    {
        var awbPage = new PageResultDto(PageNumber: 1, IsAwb: true, DocumentType: "Air Waybill", Data: BuildAwbData());
        var response = BuildResponse(totalPages: 1, pages: [awbPage]);

        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
               .ReturnsAsync(response);

        await _sut.ProcesarDocumentoAsync(1);

        _repo.Verify(r => r.GuardarResultadoOcrAsync(1, 1, It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(17));
    }

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenResultHasTwoAwbPages_ShouldGuardar34Fields()
    {
        var pages = new[]
        {
            new PageResultDto(PageNumber: 1, IsAwb: true, DocumentType: "Air Waybill", Data: BuildAwbData()),
            new PageResultDto(PageNumber: 2, IsAwb: true, DocumentType: "Air Waybill", Data: BuildAwbData()),
        };
        var response = BuildResponse(totalPages: 2, pages: pages);

        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
               .ReturnsAsync(response);

        await _sut.ProcesarDocumentoAsync(1);

        _repo.Verify(r => r.GuardarResultadoOcrAsync(1, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(34));
    }

    // ── Páginas no-AWB → no guarda campos ───────────────────────────────────

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenNoAwbPages_ShouldNotGuardarResultados()
    {
        var pages = new[]
        {
            new PageResultDto(PageNumber: 1, IsAwb: false, DocumentType: "Contract", Data: null),
        };
        var response = BuildResponse(totalPages: 1, pages: pages);

        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
               .ReturnsAsync(response);

        await _sut.ProcesarDocumentoAsync(1);

        _repo.Verify(r => r.GuardarResultadoOcrAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    // ── Contadores de páginas ────────────────────────────────────────────────

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenResultHasPages_ShouldActualizarContadores()
    {
        var pages = new[]
        {
            new PageResultDto(PageNumber: 1, IsAwb: true,  DocumentType: "Air Waybill", Data: BuildAwbData()),
            new PageResultDto(PageNumber: 2, IsAwb: false, DocumentType: "Contract",    Data: null),
        };
        var response = BuildResponse(totalPages: 3, pages: pages);

        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
               .ReturnsAsync(response);

        await _sut.ProcesarDocumentoAsync(1);

        // totalHojas=3, hojasValidas=1 (solo la página IsAwb=true)
        _repo.Verify(r => r.ActualizarContadoresPaginasAsync(1, 3, 1), Times.Once);
    }

    // ── MIME type derivado del nombre de archivo ─────────────────────────────

    [Theory]
    [InlineData("guia.pdf", "application/pdf")]
    [InlineData("scan.png", "image/png")]
    [InlineData("scan.jpg", "image/jpeg")]
    [InlineData("scan.jpeg", "image/jpeg")]
    [InlineData("unknown", "application/pdf")]  // extensión desconocida → PDF por defecto
    public async Task ProcesarDocumentoAsync_ShouldPassCorrectMimeTypeToGemini(string fileName, string expectedMime)
    {
        var solicitudInfo = new ExtractionDetailDto(1, "Pendiente", fileName, 0, 0, DateTime.UtcNow, []);
        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync(solicitudInfo);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), expectedMime))
               .ReturnsAsync((DocumentProcessResponseDto?)null);

        await _sut.ProcesarDocumentoAsync(1);

        _gemini.Verify(g => g.ProcessDocumentAsync(FakeFileBytes, expectedMime), Times.Once);
    }

    // ── Manejo de errores ────────────────────────────────────────────────────

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenRepositoryThrows_ShouldSetEstadoErrorAndRethrow()
    {
        _repo.Setup(r => r.ObtenerArchivoPorIdAsync(1))
             .ThrowsAsync(new InvalidOperationException("DB error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ProcesarDocumentoAsync(1));

        _repo.Verify(r => r.ActualizarEstadoAsync(1, "Error"), Times.Once);
    }

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenGeminiThrows_ShouldSetEstadoErrorAndRethrow()
    {
        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
               .ThrowsAsync(new HttpRequestException("Gemini unavailable"));

        await Assert.ThrowsAsync<HttpRequestException>(() => _sut.ProcesarDocumentoAsync(1));

        _repo.Verify(r => r.ActualizarEstadoAsync(1, "Error"), Times.Once);
        _repo.Verify(r => r.ActualizarEstadoAsync(1, "Finalizado"), Times.Never);
    }

    [Fact]
    public async Task ProcesarDocumentoAsync_WhenGeminiThrows_ShouldNotEliminarArchivo()
    {
        _repo.Setup(r => r.ObtenerResultadoCompletoAsync(1)).ReturnsAsync((ExtractionDetailDto?)null);
        _gemini.Setup(g => g.ProcessDocumentAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
               .ThrowsAsync(new HttpRequestException("Gemini unavailable"));

        await Assert.ThrowsAsync<HttpRequestException>(() => _sut.ProcesarDocumentoAsync(1));

        _repo.Verify(r => r.EliminarArchivoBinarioAsync(It.IsAny<int>()), Times.Never);
    }
}
