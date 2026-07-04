using DynamicFormBuilder.Business.Models;

namespace DynamicFormBuilder.Business.Services;

public interface IFormBuilderService
{
    Task<FormDetailDto?> GetBuilderDataAsync(int formId);
    Task<ServiceResult> SaveBuilderAsync(FormBuilderSaveDto dto);
}
