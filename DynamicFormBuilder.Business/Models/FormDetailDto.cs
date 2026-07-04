using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Models;

public class FormDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsPublished { get; set; }
    public FormTheme Theme { get; set; }
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#6b8cff";
    public string BackgroundColor { get; set; } = "#eef2ff";
    public string ButtonColor { get; set; } = "#6b8cff";
    public bool EmailNotificationEnabled { get; set; }
    public string? NotificationEmail { get; set; }
    public DateTime? PublishedAt { get; set; }
    public List<FormFieldDto> Fields { get; set; } = new();
    public List<ConditionalLogicDto> ConditionalLogics { get; set; } = new();
}
