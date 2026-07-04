using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Entity.Entities;

public class FormResponseValue : BaseEntity
{
    public int ResponseId { get; set; }
    public int FieldId { get; set; }
    public string FieldLabel { get; set; } = string.Empty;
    public FieldType FieldType { get; set; }
    public string? Value { get; set; }

    public FormResponse Response { get; set; } = null!;
    public FormField Field { get; set; } = null!;
}
