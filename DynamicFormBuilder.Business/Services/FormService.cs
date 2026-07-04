using DynamicFormBuilder.Business.Helpers;
using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.DataAccess.Repositories;
using DynamicFormBuilder.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.Business.Services;

public class FormService : IFormService
{
    private readonly IFormRepository _formRepository;
    private readonly IResponseRepository _responseRepository;

    public FormService(IFormRepository formRepository, IResponseRepository responseRepository)
    {
        _formRepository = formRepository;
        _responseRepository = responseRepository;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var recentForms = await _formRepository.GetRecentFormsAsync(5);
        var recentResponses = await _responseRepository.GetRecentResponsesAsync(5);

        var formItems = new List<FormListItemDto>();
        foreach (var form in recentForms)
        {
            formItems.Add(await MapToListItemAsync(form));
        }

        return new DashboardDto
        {
            TotalForms = await _formRepository.GetTotalCountAsync(),
            ActiveForms = await _formRepository.GetActiveCountAsync(),
            TotalResponses = await _responseRepository.GetTotalCountAsync(),
            RecentForms = formItems,
            RecentResponses = recentResponses.Select(r => new ResponseListItemDto
            {
                Id = r.Id,
                FormId = r.FormId,
                FormTitle = r.Form?.Title ?? string.Empty,
                SubmittedAt = r.SubmittedAt,
                IpAddress = r.IpAddress,
                ValueCount = r.Values?.Count ?? 0
            }).ToList()
        };
    }

    public async Task<IReadOnlyList<FormListItemDto>> GetAllFormsAsync()
    {
        var forms = await _formRepository.Query()
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        var result = new List<FormListItemDto>();
        foreach (var form in forms)
        {
            result.Add(await MapToListItemAsync(form));
        }

        return result;
    }

    public async Task<FormDetailDto?> GetFormDetailAsync(int id)
    {
        var form = await _formRepository.GetByIdWithDetailsAsync(id);
        return form == null ? null : EntityMapper.ToDetailDto(form);
    }

    public async Task<ServiceResult<int>> CreateFormAsync(FormEditDto dto)
    {
        var slug = await EnsureUniqueSlugAsync(dto.Slug, dto.Title);

        var form = new DynamicForm
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Slug = slug,
            IsActive = dto.IsActive,
            Theme = dto.Theme,
            LogoUrl = dto.LogoUrl,
            PrimaryColor = string.IsNullOrWhiteSpace(dto.PrimaryColor) ? "#6b8cff" : dto.PrimaryColor,
            BackgroundColor = string.IsNullOrWhiteSpace(dto.BackgroundColor) ? "#eef2ff" : dto.BackgroundColor,
            ButtonColor = string.IsNullOrWhiteSpace(dto.ButtonColor) ? "#6b8cff" : dto.ButtonColor,
            EmailNotificationEnabled = dto.EmailNotificationEnabled,
            NotificationEmail = dto.NotificationEmail?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _formRepository.AddAsync(form);
        await _formRepository.SaveChangesAsync();

        return ServiceResult<int>.Ok(form.Id, "Form başarıyla oluşturuldu.");
    }

    public async Task<ServiceResult> UpdateFormAsync(FormEditDto dto)
    {
        var form = await _formRepository.GetByIdAsync(dto.Id);
        if (form == null)
        {
            return ServiceResult.Fail("Form bulunamadı.");
        }

        var slug = string.IsNullOrWhiteSpace(dto.Slug)
            ? FormValidationHelper.GenerateSlug(dto.Title)
            : dto.Slug.Trim().ToLowerInvariant();

        if (await _formRepository.SlugExistsAsync(slug, dto.Id))
        {
            slug = await EnsureUniqueSlugAsync(slug, dto.Title);
        }

        form.Title = dto.Title.Trim();
        form.Description = dto.Description?.Trim();
        form.Slug = slug;
        form.IsActive = dto.IsActive;
        form.Theme = dto.Theme;
        form.LogoUrl = dto.LogoUrl;
        form.PrimaryColor = string.IsNullOrWhiteSpace(dto.PrimaryColor) ? "#6b8cff" : dto.PrimaryColor;
        form.BackgroundColor = string.IsNullOrWhiteSpace(dto.BackgroundColor) ? "#eef2ff" : dto.BackgroundColor;
        form.ButtonColor = string.IsNullOrWhiteSpace(dto.ButtonColor) ? "#6b8cff" : dto.ButtonColor;
        form.EmailNotificationEnabled = dto.EmailNotificationEnabled;
        form.NotificationEmail = dto.NotificationEmail?.Trim();
        form.UpdatedAt = DateTime.UtcNow;

        _formRepository.Update(form);
        await _formRepository.SaveChangesAsync();

        return ServiceResult.Ok("Form başarıyla güncellendi.");
    }

    public async Task<ServiceResult> DeleteFormAsync(int id)
    {
        var form = await _formRepository.GetByIdAsync(id);
        if (form == null)
        {
            return ServiceResult.Fail("Form bulunamadı.");
        }

        _formRepository.Remove(form);
        await _formRepository.SaveChangesAsync();

        return ServiceResult.Ok("Form silindi.");
    }

    public async Task<ServiceResult<int>> CopyFormAsync(int id)
    {
        var source = await _formRepository.GetByIdWithDetailsAsync(id);
        if (source == null)
        {
            return ServiceResult<int>.Fail("Kopyalanacak form bulunamadı.");
        }

        var newTitle = $"{source.Title} (Kopya)";
        var slug = await EnsureUniqueSlugAsync(string.Empty, newTitle);

        var copy = new DynamicForm
        {
            Title = newTitle,
            Description = source.Description,
            Slug = slug,
            IsActive = false,
            IsPublished = false,
            Theme = source.Theme,
            LogoUrl = source.LogoUrl,
            PrimaryColor = source.PrimaryColor,
            BackgroundColor = source.BackgroundColor,
            ButtonColor = source.ButtonColor,
            EmailNotificationEnabled = source.EmailNotificationEnabled,
            NotificationEmail = source.NotificationEmail,
            CreatedAt = DateTime.UtcNow,
            Fields = source.Fields.Select(f => new FormField
            {
                ClientId = Guid.NewGuid().ToString("N")[..12],
                FieldType = f.FieldType,
                Label = f.Label,
                Placeholder = f.Placeholder,
                DefaultValue = f.DefaultValue,
                HelpText = f.HelpText,
                IsRequired = f.IsRequired,
                IsReadOnly = f.IsReadOnly,
                IsHidden = f.IsHidden,
                Options = f.Options,
                MinLength = f.MinLength,
                MaxLength = f.MaxLength,
                MinValue = f.MinValue,
                MaxValue = f.MaxValue,
                RegexPattern = f.RegexPattern,
                Width = f.Width,
                CssClass = f.CssClass,
                SortOrder = f.SortOrder,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        var clientIdMap = source.Fields
            .Zip(copy.Fields, (oldField, newField) => new { OldId = oldField.ClientId, NewId = newField.ClientId })
            .ToDictionary(x => x.OldId, x => x.NewId);

        copy.ConditionalLogics = source.ConditionalLogics.Select(c => new ConditionalLogic
        {
            SourceFieldClientId = clientIdMap.GetValueOrDefault(c.SourceFieldClientId, c.SourceFieldClientId),
            Operator = c.Operator,
            Value = c.Value,
            TargetFieldClientId = clientIdMap.GetValueOrDefault(c.TargetFieldClientId, c.TargetFieldClientId),
            ActionType = c.ActionType,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _formRepository.AddAsync(copy);
        await _formRepository.SaveChangesAsync();

        return ServiceResult<int>.Ok(copy.Id, "Form kopyalandı.");
    }

    public async Task<ServiceResult> ToggleActiveAsync(int id)
    {
        var form = await _formRepository.GetByIdAsync(id);
        if (form == null)
        {
            return ServiceResult.Fail("Form bulunamadı.");
        }

        form.IsActive = !form.IsActive;
        form.UpdatedAt = DateTime.UtcNow;
        _formRepository.Update(form);
        await _formRepository.SaveChangesAsync();

        return ServiceResult.Ok(form.IsActive ? "Form aktif edildi." : "Form pasif edildi.");
    }

    public async Task<ServiceResult> PublishFormAsync(int id)
    {
        var form = await _formRepository.GetByIdWithDetailsAsync(id);
        if (form == null)
        {
            return ServiceResult.Fail("Form bulunamadı.");
        }

        if (!form.Fields.Any())
        {
            return ServiceResult.Fail("Yayınlamak için en az bir alan eklemelisiniz.");
        }

        form.IsPublished = true;
        form.IsActive = true;
        form.PublishedAt = DateTime.UtcNow;
        form.UpdatedAt = DateTime.UtcNow;

        _formRepository.Update(form);
        await _formRepository.SaveChangesAsync();

        return ServiceResult.Ok("Form yayınlandı.");
    }

    public async Task<ServiceResult> UnpublishFormAsync(int id)
    {
        var form = await _formRepository.GetByIdAsync(id);
        if (form == null)
        {
            return ServiceResult.Fail("Form bulunamadı.");
        }

        form.IsPublished = false;
        form.UpdatedAt = DateTime.UtcNow;
        _formRepository.Update(form);
        await _formRepository.SaveChangesAsync();

        return ServiceResult.Ok("Form yayından kaldırıldı.");
    }

    public string GetPublicUrl(string slug, string baseUrl)
    {
        return $"{baseUrl.TrimEnd('/')}/PublicForm/Fill/{slug}";
    }

    private async Task<FormListItemDto> MapToListItemAsync(DynamicForm form)
    {
        var fieldCount = await _formRepository.Query()
            .Where(f => f.Id == form.Id)
            .SelectMany(f => f.Fields)
            .CountAsync();

        return new FormListItemDto
        {
            Id = form.Id,
            Title = form.Title,
            Slug = form.Slug,
            IsActive = form.IsActive,
            IsPublished = form.IsPublished,
            Theme = form.Theme,
            CreatedAt = form.CreatedAt,
            ResponseCount = await _responseRepository.GetCountByFormIdAsync(form.Id),
            FieldCount = fieldCount
        };
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug, string title)
    {
        var baseSlug = string.IsNullOrWhiteSpace(slug)
            ? FormValidationHelper.GenerateSlug(title)
            : slug.Trim().ToLowerInvariant();

        var uniqueSlug = baseSlug;
        var counter = 1;

        while (await _formRepository.SlugExistsAsync(uniqueSlug))
        {
            uniqueSlug = $"{baseSlug}-{counter++}";
        }

        return uniqueSlug;
    }
}
