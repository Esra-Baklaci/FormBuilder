using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.Entity.Entities;
using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Helpers;

public static class EntityMapper
{
    public static FormFieldDto ToDto(FormField field) => new()
    {
        Id = field.Id,
        ClientId = field.ClientId,
        FieldType = field.FieldType,
        Label = field.Label,
        Placeholder = field.Placeholder,
        DefaultValue = field.DefaultValue,
        HelpText = field.HelpText,
        IsRequired = field.IsRequired,
        IsReadOnly = field.IsReadOnly,
        IsHidden = field.IsHidden,
        Options = field.Options,
        MinLength = field.MinLength,
        MaxLength = field.MaxLength,
        MinValue = field.MinValue,
        MaxValue = field.MaxValue,
        RegexPattern = field.RegexPattern,
        Width = field.Width,
        CssClass = field.CssClass,
        SortOrder = field.SortOrder
    };

    public static ConditionalLogicDto ToDto(ConditionalLogic logic) => new()
    {
        Id = logic.Id,
        SourceFieldClientId = logic.SourceFieldClientId,
        Operator = logic.Operator,
        Value = logic.Value,
        TargetFieldClientId = logic.TargetFieldClientId,
        ActionType = logic.ActionType
    };

    public static FormDetailDto ToDetailDto(DynamicForm form) => new()
    {
        Id = form.Id,
        Title = form.Title,
        Description = form.Description,
        Slug = form.Slug,
        IsActive = form.IsActive,
        IsPublished = form.IsPublished,
        Theme = form.Theme,
        LogoUrl = form.LogoUrl,
        PrimaryColor = form.PrimaryColor,
        BackgroundColor = form.BackgroundColor,
        ButtonColor = form.ButtonColor,
        EmailNotificationEnabled = form.EmailNotificationEnabled,
        NotificationEmail = form.NotificationEmail,
        PublishedAt = form.PublishedAt,
        Fields = form.Fields.OrderBy(f => f.SortOrder).Select(ToDto).ToList(),
        ConditionalLogics = form.ConditionalLogics.Select(ToDto).ToList()
    };

    public static FormField ToEntity(FormFieldDto dto, int formId)
    {
        return new FormField
        {
            Id = dto.Id ?? 0,
            FormId = formId,
            ClientId = dto.ClientId,
            FieldType = dto.FieldType,
            Label = dto.Label,
            Placeholder = dto.Placeholder,
            DefaultValue = dto.DefaultValue,
            HelpText = dto.HelpText,
            IsRequired = dto.IsRequired,
            IsReadOnly = dto.IsReadOnly,
            IsHidden = dto.IsHidden,
            Options = dto.Options,
            MinLength = dto.MinLength,
            MaxLength = dto.MaxLength,
            MinValue = dto.MinValue,
            MaxValue = dto.MaxValue,
            RegexPattern = dto.RegexPattern,
            Width = dto.Width,
            CssClass = dto.CssClass,
            SortOrder = dto.SortOrder
        };
    }
}
