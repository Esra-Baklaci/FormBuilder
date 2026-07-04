using DynamicFormBuilder.Entity.Entities;

namespace DynamicFormBuilder.DataAccess.Repositories;

public interface IFormRepository : IGenericRepository<DynamicForm>
{
    Task<DynamicForm?> GetByIdWithDetailsAsync(int id);
    Task<DynamicForm?> GetBySlugAsync(string slug);
    Task<IReadOnlyList<DynamicForm>> GetRecentFormsAsync(int count);
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}
