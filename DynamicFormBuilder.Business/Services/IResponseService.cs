using DynamicFormBuilder.Business.Models;

namespace DynamicFormBuilder.Business.Services;

public interface IResponseService
{
    Task<IReadOnlyList<ResponseListItemDto>> GetResponsesByFormIdAsync(int formId);
    Task<ResponseDetailDto?> GetResponseDetailAsync(int id);
    Task<ServiceResult> DeleteResponseAsync(int id);
    Task<byte[]> ExportToExcelAsync(int formId);
}
