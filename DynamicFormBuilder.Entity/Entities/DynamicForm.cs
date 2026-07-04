using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Entity.Entities;

public class DynamicForm : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsPublished { get; set; }
    public FormTheme Theme { get; set; } = FormTheme.Light;
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#6b8cff";
    public string BackgroundColor { get; set; } = "#eef2ff";
    public string ButtonColor { get; set; } = "#6b8cff";
    public bool EmailNotificationEnabled { get; set; }
    public string? NotificationEmail { get; set; }
    public DateTime? PublishedAt { get; set; }

    public ICollection<FormField> Fields { get; set; } = new List<FormField>();
    public ICollection<FormResponse> Responses { get; set; } = new List<FormResponse>();
    public ICollection<ConditionalLogic> ConditionalLogics { get; set; } = new List<ConditionalLogic>();
}
