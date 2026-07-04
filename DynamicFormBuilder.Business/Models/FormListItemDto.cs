using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Models;

public class FormListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsPublished { get; set; }
    public FormTheme Theme { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ResponseCount { get; set; }
    public int FieldCount { get; set; }
}
