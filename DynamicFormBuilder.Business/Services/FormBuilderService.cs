using DynamicFormBuilder.Business.Helpers;
using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.DataAccess.Repositories;
using DynamicFormBuilder.Entity.Entities;

namespace DynamicFormBuilder.Business.Services;

public class FormBuilderService : IFormBuilderService
{
    private readonly IFormRepository _formRepository;

    public FormBuilderService(IFormRepository formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<FormDetailDto?> GetBuilderDataAsync(int formId)
    {
        var form = await _formRepository.GetByIdWithDetailsAsync(formId);
        return form == null ? null : EntityMapper.ToDetailDto(form);
    }

    public async Task<ServiceResult> SaveBuilderAsync(FormBuilderSaveDto dto)
    {
        var form = await _formRepository.GetByIdWithDetailsAsync(dto.FormId);
        if (form == null)
        {
            return ServiceResult.Fail("Form bulunamadı.");
        }

        if (dto.Fields.GroupBy(f => f.ClientId).Any(g => g.Count() > 1))
        {
            return ServiceResult.Fail("Aynı ClientId'ye sahip alanlar bulunuyor.");
        }

        for (var i = 0; i < dto.Fields.Count; i++)
        {
            dto.Fields[i].SortOrder = i;
        }

        var existingFields = form.Fields.ToList();
        var incomingClientIds = dto.Fields.Select(f => f.ClientId).ToHashSet();

        var fieldsToRemove = existingFields.Where(f => !incomingClientIds.Contains(f.ClientId)).ToList();
        foreach (var field in fieldsToRemove)
        {
            form.Fields.Remove(field);
        }

        foreach (var fieldDto in dto.Fields)
        {
            if (string.IsNullOrWhiteSpace(fieldDto.ClientId))
            {
                fieldDto.ClientId = Guid.NewGuid().ToString("N")[..12];
            }

            var existing = existingFields.FirstOrDefault(f => f.ClientId == fieldDto.ClientId);
            if (existing != null)
            {
                existing.FieldType = fieldDto.FieldType;
                existing.Label = fieldDto.Label;
                existing.Placeholder = fieldDto.Placeholder;
                existing.DefaultValue = fieldDto.DefaultValue;
                existing.HelpText = fieldDto.HelpText;
                existing.IsRequired = fieldDto.IsRequired;
                existing.IsReadOnly = fieldDto.IsReadOnly;
                existing.IsHidden = fieldDto.IsHidden;
                existing.Options = fieldDto.Options;
                existing.MinLength = fieldDto.MinLength;
                existing.MaxLength = fieldDto.MaxLength;
                existing.MinValue = fieldDto.MinValue;
                existing.MaxValue = fieldDto.MaxValue;
                existing.RegexPattern = fieldDto.RegexPattern;
                existing.Width = fieldDto.Width;
                existing.CssClass = fieldDto.CssClass;
                existing.SortOrder = fieldDto.SortOrder;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                form.Fields.Add(EntityMapper.ToEntity(fieldDto, form.Id));
            }
        }

        form.ConditionalLogics.Clear();
        foreach (var logicDto in dto.ConditionalLogics)
        {
            var sourceField = form.Fields.FirstOrDefault(f => f.ClientId == logicDto.SourceFieldClientId);
            var targetField = form.Fields.FirstOrDefault(f => f.ClientId == logicDto.TargetFieldClientId);

            form.ConditionalLogics.Add(new ConditionalLogic
            {
                FormId = form.Id,
                SourceFieldClientId = logicDto.SourceFieldClientId,
                SourceFieldId = sourceField?.Id,
                Operator = logicDto.Operator,
                Value = logicDto.Value,
                TargetFieldClientId = logicDto.TargetFieldClientId,
                TargetFieldId = targetField?.Id,
                ActionType = logicDto.ActionType,
                CreatedAt = DateTime.UtcNow
            });
        }

        form.UpdatedAt = DateTime.UtcNow;
        _formRepository.Update(form);
        await _formRepository.SaveChangesAsync();

        return ServiceResult.Ok("Form tasarımı kaydedildi.");
    }
}
