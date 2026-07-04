using DynamicFormBuilder.Business.Models;

namespace DynamicFormBuilder.Business.Services;

public interface IFormService
{
    Task<DashboardDto> GetDashboardAsync();
    Task<IReadOnlyList<FormListItemDto>> GetAllFormsAsync();
    Task<FormDetailDto?> GetFormDetailAsync(int id);
    Task<ServiceResult<int>> CreateFormAsync(FormEditDto dto);
    Task<ServiceResult> UpdateFormAsync(FormEditDto dto);
    Task<ServiceResult> DeleteFormAsync(int id);
    Task<ServiceResult<int>> CopyFormAsync(int id);
    Task<ServiceResult> ToggleActiveAsync(int id);
    Task<ServiceResult> PublishFormAsync(int id);
    Task<ServiceResult> UnpublishFormAsync(int id);
    string GetPublicUrl(string slug, string baseUrl);
}
