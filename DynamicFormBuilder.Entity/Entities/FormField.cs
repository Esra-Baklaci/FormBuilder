using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Entity.Entities;

public class FormField : BaseEntity
{
    public int FormId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public FieldType FieldType { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Placeholder { get; set; }
    public string? DefaultValue { get; set; }
    public string? HelpText { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsHidden { get; set; }
    public string? Options { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public string? RegexPattern { get; set; }
    public string Width { get; set; } = "100%";
    public string? CssClass { get; set; }
    public int SortOrder { get; set; }

    public DynamicForm Form { get; set; } = null!;
}
