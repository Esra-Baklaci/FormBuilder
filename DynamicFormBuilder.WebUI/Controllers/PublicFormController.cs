using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.WebUI.Controllers;

public class PublicFormController : Controller
{
    private readonly IPublicFormService _publicFormService;
    private readonly IConfiguration _configuration;

    public PublicFormController(IPublicFormService publicFormService, IConfiguration configuration)
    {
        _publicFormService = publicFormService;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("PublicForm/Fill/{slug}")]
    public async Task<IActionResult> Fill(string slug)
    {
        var form = await _publicFormService.GetPublishedFormAsync(slug);
        if (form == null)
        {
            return View("NotAvailable");
        }

        return View(form);
    }

    [HttpPost]
    [Route("PublicForm/Fill/{slug}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Fill(string slug, IFormCollection formCollection)
    {
        var form = await _publicFormService.GetPublishedFormAsync(slug);
        if (form == null)
        {
            return View("NotAvailable");
        }

        var values = new Dictionary<string, string?>();
        foreach (var key in formCollection.Keys.Where(k => !k.StartsWith("__")))
        {
            values[key] = Request.Form[key].ToString();
        }

        var fileUploads = new Dictionary<string, FileUploadItem>();
        var maxFileSize = long.Parse(_configuration["FileUpload:MaxFileSizeBytes"] ?? "5242880");
        var allowedExtensions = (_configuration["FileUpload:AllowedExtensions"] ?? ".jpg,.jpeg,.png,.gif,.pdf")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(e => e.ToLowerInvariant())
            .ToHashSet();

        foreach (var file in Request.Form.Files)
        {
            if (file.Length == 0)
            {
                continue;
            }

            if (file.Length > maxFileSize)
            {
                ModelState.AddModelError(string.Empty, $"{file.Name} dosyası çok büyük.");
                continue;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError(string.Empty, $"{file.FileName} dosya türüne izin verilmiyor.");
                continue;
            }

            fileUploads[file.Name] = new FileUploadItem
            {
                Stream = file.OpenReadStream(),
                FileName = file.FileName
            };
        }

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _publicFormService.SubmitFormAsync(slug, values, ipAddress, fileUploads);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Gönderim başarısız.");
            return View(form);
        }

        return View("ThankYou", form);
    }
}
