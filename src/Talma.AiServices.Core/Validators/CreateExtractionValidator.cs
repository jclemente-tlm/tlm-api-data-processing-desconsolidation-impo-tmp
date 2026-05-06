using FluentValidation;
using Talma.AiServices.Core.DTOs;
using System.IO;
using System.Linq;

namespace Talma.AiServices.Core.Validators;

public class CreateExtractionValidator : AbstractValidator<OcrRequestDto>
{
    private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

    public CreateExtractionValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("El nombre del archivo es obligatorio")
            .MaximumLength(100).WithMessage("El nombre es muy largo")
            .Must(HaveAllowedExtension).WithMessage("Tipo de archivo no permitido. Solo se aceptan PDF o imágenes (JPG, PNG).");

        // Regla 1: Base64 decodificable y con bytes suficientes
        RuleFor(x => x.FileBase64)
            .NotEmpty().WithMessage("El contenido Base64 es requerido")
            .Must(BeDecodable).WithMessage("El archivo está corrupto o no puede ser procesado.");

        // Regla 2: No es Word
        RuleFor(x => x)
            .Must(x => !IsWordDocument(x.FileBase64, x.FileName))
            .WithMessage("El archivo es un documento Word, no está permitido. Solo se aceptan PDF o imágenes (JPG, PNG).")
            .When(x => BeDecodable(x.FileBase64));

        // Regla 3: No es Excel
        RuleFor(x => x)
            .Must(x => !IsExcelDocument(x.FileBase64, x.FileName))
            .WithMessage("El archivo es un documento Excel, no está permitido. Solo se aceptan PDF o imágenes (JPG, PNG).")
            .When(x => BeDecodable(x.FileBase64));

        // Regla 4: Es un formato permitido (PDF, JPG, PNG)
        RuleFor(x => x.FileBase64)
            .Must(BeAValidFileFormat).WithMessage("El archivo no tiene un formato válido o está corrupto.")
            .When(x => BeDecodable(x.FileBase64) && !IsWordDocument(x.FileBase64, x.FileName) && !IsExcelDocument(x.FileBase64, x.FileName));

        // Regla 5: PDF no debe estar protegido con contraseña
        RuleFor(x => x.FileBase64)
            .Must(base64 => !IsPasswordProtectedPdf(base64)).WithMessage("El archivo PDF está protegido con contraseña, no está permitido.")
            .When(x => BeDecodable(x.FileBase64) && IsPdf(x.FileBase64));
    }

    private bool HaveAllowedExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }

    private bool BeDecodable(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return false;
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return bytes.Length >= 4;
        }
        catch { return false; }
    }

    private bool IsWordDocument(string base64, string fileName)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();

            // .doc → OLE2: D0 CF 11 E0 (sin extensión Excel)
            bool isOle2Word = bytes[0] == 0xD0 && bytes[1] == 0xCF && bytes[2] == 0x11 && bytes[3] == 0xE0
                              && ext != ".xls" && ext != ".xlsx";

            // .docx → ZIP: busca rutas internas de Word ("word/") en el contenido
            bool isZipWord = bytes[0] == 0x50 && bytes[1] == 0x4B && bytes[2] == 0x03 && bytes[3] == 0x04
                             && ContainsSequence(bytes, System.Text.Encoding.ASCII.GetBytes("word/"));

            return isOle2Word || isZipWord;
        }
        catch { return false; }
    }

    private bool IsExcelDocument(string base64, string fileName)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();

            // .xls → OLE2: D0 CF 11 E0 con extensión Excel
            bool isOle2Excel = bytes[0] == 0xD0 && bytes[1] == 0xCF && bytes[2] == 0x11 && bytes[3] == 0xE0
                               && (ext == ".xls" || ext == ".xlsx");

            // .xlsx → ZIP: busca rutas internas de Excel ("xl/") en el contenido
            bool isZipExcel = bytes[0] == 0x50 && bytes[1] == 0x4B && bytes[2] == 0x03 && bytes[3] == 0x04
                              && ContainsSequence(bytes, System.Text.Encoding.ASCII.GetBytes("xl/workbook"));

            return isOle2Excel || isZipExcel;
        }
        catch { return false; }
    }

    private bool ContainsSequence(byte[] data, byte[] sequence)
    {
        for (int i = 0; i <= data.Length - sequence.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < sequence.Length; j++)
            {
                if (data[i + j] != sequence[j]) { match = false; break; }
            }
            if (match) return true;
        }
        return false;
    }

    // Validación de Magic Numbers para formatos permitidos
    private bool BeAValidFileFormat(string base64)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);

            // PDF: %PDF (25 50 44 46)
            bool isPdf = bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46;

            // PNG: (89 50 4E 47)
            bool isPng = bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;

            // JPEG: (FF D8 FF)
            bool isJpeg = bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;

            return isPdf || isPng || isJpeg;
        }
        catch { return false; }
    }

    private bool IsPdf(string base64)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return bytes.Length >= 4
                && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46;
        }
        catch { return false; }
    }

    private bool IsPasswordProtectedPdf(string base64)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            // Busca el token "/Encrypt" en el contenido del PDF
            byte[] token = System.Text.Encoding.ASCII.GetBytes("/Encrypt");
            for (int i = 0; i <= bytes.Length - token.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < token.Length; j++)
                {
                    if (bytes[i + j] != token[j]) { match = false; break; }
                }
                if (match) return true;
            }
            return false;
        }
        catch { return false; }
    }
}