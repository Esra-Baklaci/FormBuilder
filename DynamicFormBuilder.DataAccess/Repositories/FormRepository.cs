using DynamicFormBuilder.DataAccess.Context;
using DynamicFormBuilder.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.DataAccess.Repositories;

public class FormRepository : GenericRepository<DynamicForm>, IFormRepository
{
    public FormRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<DynamicForm?> GetByIdWithDetailsAsync(int id)
    {
        return await Context.Forms
            .Include(f => f.Fields.OrderBy(ff => ff.SortOrder))
            .Include(f => f.ConditionalLogics)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<DynamicForm?> GetBySlugAsync(string slug)
    {
        return await Context.Forms
            .Include(f => f.Fields.OrderBy(ff => ff.SortOrder))
            .Include(f => f.ConditionalLogics)
            .FirstOrDefaultAsync(f => f.Slug == slug);
    }

    public async Task<IReadOnlyList<DynamicForm>> GetRecentFormsAsync(int count)
    {
        return await Context.Forms
            .OrderByDescending(f => f.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public Task<int> GetTotalCountAsync()
    {
        return Context.Forms.CountAsync();
    }

    public Task<int> GetActiveCountAsync()
    {
        return Context.Forms.CountAsync(f => f.IsActive);
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = Context.Forms.Where(f => f.Slug == slug);
        if (excludeId.HasValue)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
