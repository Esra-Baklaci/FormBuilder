using System.Text.RegularExpressions;
using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Helpers;

public static class FormValidationHelper
{
    private static readonly HashSet<FieldType> InputFieldTypes = new()
    {
        FieldType.Textbox, FieldType.Textarea, FieldType.Number, FieldType.Email,
        FieldType.Phone, FieldType.Date, FieldType.Time, FieldType.Password,
        FieldType.HiddenField, FieldType.Dropdown, FieldType.RadioButton,
        FieldType.Checkbox, FieldType.MultiSelect, FieldType.FileUpload,
        FieldType.ImageUpload, FieldType.Signature, FieldType.Rating, FieldType.Slider
    };

    public static bool IsInputField(FieldType fieldType) => InputFieldTypes.Contains(fieldType);

    public static List<string> ValidateFieldValue(FormFieldDto field, string? value)
    {
        var errors = new List<string>();

        if (!IsInputField(field.FieldType) || field.IsHidden)
        {
            return errors;
        }

        if (field.IsRequired && string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{field.Label} alanı zorunludur.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return errors;
        }

        if (field.MinLength.HasValue && value.Length < field.MinLength.Value)
        {
            errors.Add($"{field.Label} en az {field.MinLength} karakter olmalıdır.");
        }

        if (field.MaxLength.HasValue && value.Length > field.MaxLength.Value)
        {
            errors.Add($"{field.Label} en fazla {field.MaxLength} karakter olmalıdır.");
        }

        if (field.FieldType == FieldType.Email && !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            errors.Add($"{field.Label} geçerli bir e-posta adresi olmalıdır.");
        }

        if (field.FieldType == FieldType.Number && decimal.TryParse(value, out var numValue))
        {
            if (field.MinValue.HasValue && numValue < field.MinValue.Value)
            {
                errors.Add($"{field.Label} en az {field.MinValue} olmalıdır.");
            }

            if (field.MaxValue.HasValue && numValue > field.MaxValue.Value)
            {
                errors.Add($"{field.Label} en fazla {field.MaxValue} olmalıdır.");
            }
        }

        if (!string.IsNullOrWhiteSpace(field.RegexPattern))
        {
            try
            {
                if (!Regex.IsMatch(value, field.RegexPattern))
                {
                    errors.Add($"{field.Label} geçerli formatta değil.");
                }
            }
            catch (RegexParseException)
            {
                errors.Add($"{field.Label} için regex deseni geçersiz.");
            }
        }

        return errors;
    }

    public static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? Guid.NewGuid().ToString("N")[..8] : slug;
    }
}
