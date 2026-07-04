using DynamicFormBuilder.Business.Models;

namespace DynamicFormBuilder.Business.Services;

public interface IPublicFormService
{
    Task<FormDetailDto?> GetPublishedFormAsync(string slug);
    Task<ServiceResult<int>> SubmitFormAsync(string slug, Dictionary<string, string?> values, string? ipAddress, IReadOnlyDictionary<string, FileUploadItem> fileUploads);
}
