using Talma.AiServices.Core.DTOs;
using Talma.AiServices.Core.Validators;
using Xunit;

namespace Talma.AiServices.Core.Tests.Validators;

public class CreateExtractionValidatorTests
{
    private readonly CreateExtractionValidator _sut = new();

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static string ToBase64(byte[] bytes) => Convert.ToBase64String(bytes);

    private static string ValidPdfBase64() =>
        ToBase64([0x25, 0x50, 0x44, 0x46, 0x00, 0x00, 0x00, 0x00]);   // %PDF

    private static string ValidPngBase64() =>
        ToBase64([0x89, 0x50, 0x4E, 0x47, 0x00, 0x00, 0x00, 0x00]);   // PNG magic

    private static string ValidJpegBase64() =>
        ToBase64([0xFF, 0xD8, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00]);   // JPEG magic

    private static string WordDocxBase64()
    {
        // ZIP magic (PK\x03\x04) + "word/" content → detected as Word
        byte[] zipMagic = [0x50, 0x4B, 0x03, 0x04];
        byte[] content = System.Text.Encoding.ASCII.GetBytes("word/document.xml");
        return ToBase64([.. zipMagic, .. new byte[10], .. content]);
    }

    private static string ExcelXlsxBase64()
    {
        // ZIP magic (PK\x03\x04) + "xl/workbook" content → detected as Excel
        byte[] zipMagic = [0x50, 0x4B, 0x03, 0x04];
        byte[] content = System.Text.Encoding.ASCII.GetBytes("xl/workbook.xml");
        return ToBase64([.. zipMagic, .. new byte[10], .. content]);
    }

    private static string PasswordProtectedPdfBase64()
    {
        // PDF magic + "/Encrypt" token → rejected
        byte[] pdfMagic = [0x25, 0x50, 0x44, 0x46];
        byte[] encryptToken = System.Text.Encoding.ASCII.GetBytes("/Encrypt");
        return ToBase64([.. pdfMagic, .. new byte[20], .. encryptToken]);
    }

    private static string RandomBytesBase64() =>
        ToBase64([0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07]);

    private static OcrRequestDto BuildRequest(string fileName, string fileBase64) =>
        new() { FileName = fileName, FileBase64 = fileBase64 };

    // ── FileName validations ─────────────────────────────────────────────────

    [Fact]
    public void Validate_WhenFileNameIsEmpty_ShouldFail()
    {
        var result = _sut.Validate(BuildRequest("", ValidPdfBase64()));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(OcrRequestDto.FileName));
    }

    [Fact]
    public void Validate_WhenFileNameExceeds100Chars_ShouldFail()
    {
        var longName = new string('a', 97) + ".pdf";
        var result = _sut.Validate(BuildRequest(longName, ValidPdfBase64()));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(OcrRequestDto.FileName));
    }

    [Theory]
    [InlineData("file.txt")]
    [InlineData("file.docx")]
    [InlineData("file.zip")]
    [InlineData("file.xlsx")]
    public void Validate_WhenExtensionNotAllowed_ShouldFail(string fileName)
    {
        var result = _sut.Validate(BuildRequest(fileName, ValidPdfBase64()));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(OcrRequestDto.FileName));
    }

    // ── FileBase64 validations ───────────────────────────────────────────────

    [Fact]
    public void Validate_WhenFileBase64IsEmpty_ShouldFail()
    {
        var result = _sut.Validate(BuildRequest("file.pdf", ""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(OcrRequestDto.FileBase64));
    }

    [Fact]
    public void Validate_WhenFileBase64IsNotDecodable_ShouldFail()
    {
        var result = _sut.Validate(BuildRequest("file.pdf", "not-valid-base64!!!"));
        Assert.False(result.IsValid);
    }

    // ── Valid formats ────────────────────────────────────────────────────────

    [Fact]
    public void Validate_WhenValidPdf_ShouldPass()
    {
        var result = _sut.Validate(BuildRequest("guia.pdf", ValidPdfBase64()));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenValidPng_ShouldPass()
    {
        var result = _sut.Validate(BuildRequest("scan.png", ValidPngBase64()));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenValidJpeg_ShouldPass()
    {
        var result = _sut.Validate(BuildRequest("scan.jpg", ValidJpegBase64()));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenValidJpegWithJpegExtension_ShouldPass()
    {
        var result = _sut.Validate(BuildRequest("scan.jpeg", ValidJpegBase64()));
        Assert.True(result.IsValid);
    }

    // ── Forbidden formats ────────────────────────────────────────────────────

    [Fact]
    public void Validate_WhenWordDocument_ShouldFail()
    {
        var result = _sut.Validate(BuildRequest("reporte.docx", WordDocxBase64()));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Word"));
    }

    [Fact]
    public void Validate_WhenExcelDocument_ShouldFail()
    {
        var result = _sut.Validate(BuildRequest("reporte.xlsx", ExcelXlsxBase64()));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Excel"));
    }

    [Fact]
    public void Validate_WhenRandomBytes_ShouldFailFormatValidation()
    {
        var result = _sut.Validate(BuildRequest("file.pdf", RandomBytesBase64()));
        Assert.False(result.IsValid);
    }

    // ── Password protected PDF ───────────────────────────────────────────────

    [Fact]
    public void Validate_WhenPasswordProtectedPdf_ShouldFail()
    {
        var result = _sut.Validate(BuildRequest("protegido.pdf", PasswordProtectedPdfBase64()));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("contraseña"));
    }
}
