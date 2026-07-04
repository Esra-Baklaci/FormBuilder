using DynamicFormBuilder.DataAccess.Context;
using DynamicFormBuilder.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.DataAccess.Repositories;

public class ResponseRepository : GenericRepository<FormResponse>, IResponseRepository
{
    public ResponseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<FormResponse>> GetByFormIdAsync(int formId)
    {
        return await Context.FormResponses
            .Include(r => r.Values)
            .Where(r => r.FormId == formId)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }

    public async Task<FormResponse?> GetByIdWithValuesAsync(int id)
    {
        return await Context.FormResponses
            .Include(r => r.Form)
            .Include(r => r.Values)
            .ThenInclude(v => v.Field)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<FormResponse>> GetRecentResponsesAsync(int count)
    {
        return await Context.FormResponses
            .Include(r => r.Form)
            .OrderByDescending(r => r.SubmittedAt)
            .Take(count)
            .ToListAsync();
    }

    public Task<int> GetTotalCountAsync()
    {
        return Context.FormResponses.CountAsync();
    }

    public Task<int> GetCountByFormIdAsync(int formId)
    {
        return Context.FormResponses.CountAsync(r => r.FormId == formId);
    }
}
