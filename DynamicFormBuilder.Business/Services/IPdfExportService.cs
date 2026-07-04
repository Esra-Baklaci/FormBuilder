using DynamicFormBuilder.Business.Models;

namespace DynamicFormBuilder.Business.Services;

public interface IPdfExportService
{
    Task<byte[]> ExportResponseToPdfAsync(ResponseDetailDto response);
}
