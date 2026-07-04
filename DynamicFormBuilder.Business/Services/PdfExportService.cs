using System.Text;
using DynamicFormBuilder.Business.Models;
using Microsoft.Extensions.Logging;

namespace DynamicFormBuilder.Business.Services;

public class PdfExportService : IPdfExportService
{
    private readonly ILogger<PdfExportService> _logger;

    public PdfExportService(ILogger<PdfExportService> logger)
    {
        _logger = logger;
    }

    public Task<byte[]> ExportResponseToPdfAsync(ResponseDetailDto response)
    {
        _logger.LogInformation("PDF export altyapısı çağrıldı. ResponseId: {ResponseId}", response.Id);

        var content = new StringBuilder();
        content.AppendLine($"Form: {response.FormTitle}");
        content.AppendLine($"Yanıt ID: {response.Id}");
        content.AppendLine($"Gönderim: {response.SubmittedAt.ToLocalTime():g}");
        content.AppendLine(new string('-', 40));

        foreach (var value in response.Values)
        {
            content.AppendLine($"{value.FieldLabel}: {value.Value}");
        }

        content.AppendLine();
        content.AppendLine("Not: Bu dosya PDF export servis iskeleti tarafından oluşturulmuştur.");
        content.AppendLine("Üretim ortamında QuestPDF veya iTextSharp gibi bir kütüphane entegre edilebilir.");

        return Task.FromResult(Encoding.UTF8.GetBytes(content.ToString()));
    }
}
