using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Models;

public class ResponseValueDto
{
    public int FieldId { get; set; }
    public string FieldLabel { get; set; } = string.Empty;
    public FieldType FieldType { get; set; }
    public string? Value { get; set; }
}
