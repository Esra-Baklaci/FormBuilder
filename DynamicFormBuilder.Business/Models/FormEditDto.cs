using System.ComponentModel.DataAnnotations;
using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Models;

public class FormEditDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public FormTheme Theme { get; set; } = FormTheme.Light;
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#6b8cff";
    public string BackgroundColor { get; set; } = "#eef2ff";
    public string ButtonColor { get; set; } = "#6b8cff";
    public bool EmailNotificationEnabled { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    public string? NotificationEmail { get; set; }
}
