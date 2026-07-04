using DynamicFormBuilder.Business.Helpers;
using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.DataAccess.Repositories;
using DynamicFormBuilder.Entity.Entities;
using DynamicFormBuilder.Entity.Enums;
using Microsoft.Extensions.Configuration;

namespace DynamicFormBuilder.Business.Services;

public class PublicFormService : IPublicFormService
{
    private readonly IFormRepository _formRepository;
    private readonly IResponseRepository _responseRepository;
    private readonly IEmailService _emailService;
    private readonly string _uploadPath;
    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;

    public PublicFormService(
        IFormRepository formRepository,
        IResponseRepository responseRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _formRepository = formRepository;
        _responseRepository = responseRepository;
        _emailService = emailService;
        _uploadPath = configuration["FileUpload:UploadPath"] ?? "uploads";
        _maxFileSize = long.Parse(configuration["FileUpload:MaxFileSizeBytes"] ?? "5242880");
        _allowedExtensions = (configuration["FileUpload:AllowedExtensions"] ?? ".jpg,.jpeg,.png,.gif,.pdf,.doc,.docx,.xls,.xlsx,.txt")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public async Task<FormDetailDto?> GetPublishedFormAsync(string slug)
    {
        var form = await _formRepository.GetBySlugAsync(slug);
        if (form == null || !form.IsPublished || !form.IsActive)
        {
            return null;
        }

        return EntityMapper.ToDetailDto(form);
    }

    public async Task<ServiceResult<int>> SubmitFormAsync(
        string slug,
        Dictionary<string, string?> values,
        string? ipAddress,
        IReadOnlyDictionary<string, FileUploadItem> fileUploads)
    {
        var form = await _formRepository.GetBySlugAsync(slug);
        if (form == null || !form.IsPublished || !form.IsActive)
        {
            return ServiceResult<int>.Fail("Form bulunamadı veya yayında değil.");
        }

        var fieldDtos = form.Fields.OrderBy(f => f.SortOrder).Select(EntityMapper.ToDto).ToList();
        var errors = new List<string>();

        foreach (var field in fieldDtos.Where(f => FormValidationHelper.IsInputField(f.FieldType) && !f.IsHidden))
        {
            values.TryGetValue(field.ClientId, out var value);

            if (field.FieldType is FieldType.FileUpload or FieldType.ImageUpload)
            {
                if (field.IsRequired && !fileUploads.ContainsKey(field.ClientId))
                {
                    errors.Add($"{field.Label} alanı zorunludur.");
                }
                continue;
            }

            errors.AddRange(FormValidationHelper.ValidateFieldValue(field, value));
        }

        if (errors.Any())
        {
            return ServiceResult<int>.Fail(string.Join(" ", errors.Distinct()));
        }

        var response = new FormResponse
        {
            FormId = form.Id,
            SubmittedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var field in form.Fields.Where(f => FormValidationHelper.IsInputField(f.FieldType) && !f.IsHidden))
        {
            string? storedValue = null;

            if (field.FieldType is FieldType.FileUpload or FieldType.ImageUpload)
            {
                if (fileUploads.TryGetValue(field.ClientId, out var upload))
                {
                    storedValue = await SaveUploadedFileAsync(upload, field.ClientId);
                }
            }
            else
            {
                values.TryGetValue(field.ClientId, out storedValue);
            }

            response.Values.Add(new FormResponseValue
            {
                FieldId = field.Id,
                FieldLabel = field.Label,
                FieldType = field.FieldType,
                Value = storedValue,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _responseRepository.AddAsync(response);
        await _responseRepository.SaveChangesAsync();

        if (form.EmailNotificationEnabled && !string.IsNullOrWhiteSpace(form.NotificationEmail))
        {
            await _emailService.SendFormSubmissionNotificationAsync(
                form.NotificationEmail,
                form.Title,
                response.SubmittedAt,
                response.Values.Select(v => (v.FieldLabel, v.Value ?? string.Empty)).ToList());
        }

        return ServiceResult<int>.Ok(response.Id, "Formunuz başarıyla gönderildi.");
    }

    private async Task<string> SaveUploadedFileAsync(FileUploadItem upload, string clientId)
    {
        var extension = Path.GetExtension(upload.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension.ToLowerInvariant()))
        {
            throw new InvalidOperationException("Geçersiz dosya uzantısı.");
        }

        var fileName = $"{clientId}_{Guid.NewGuid():N}{extension}";
        var relativePath = Path.Combine(_uploadPath, DateTime.UtcNow.ToString("yyyy/MM"));
        var fullDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        Directory.CreateDirectory(fullDirectory);

        var fullPath = Path.Combine(fullDirectory, fileName);
        await using var fileStream = File.Create(fullPath);
        await upload.Stream.CopyToAsync(fileStream);

        return $"/{relativePath.Replace('\\', '/')}/{fileName}";
    }
}
