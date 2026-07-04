using DynamicFormBuilder.Entity.Entities;

namespace DynamicFormBuilder.DataAccess.Repositories;

public interface IResponseRepository : IGenericRepository<FormResponse>
{
    Task<IReadOnlyList<FormResponse>> GetByFormIdAsync(int formId);
    Task<FormResponse?> GetByIdWithValuesAsync(int id);
    Task<IReadOnlyList<FormResponse>> GetRecentResponsesAsync(int count);
    Task<int> GetTotalCountAsync();
    Task<int> GetCountByFormIdAsync(int formId);
}
