using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.WebUI.Controllers;

public class FormController : Controller
{
    private readonly IFormService _formService;
    private readonly IWebHostEnvironment _env;

    public FormController(IFormService formService, IWebHostEnvironment env)
    {
        _formService = formService;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var forms = await _formService.GetAllFormsAsync();
        return View(forms);
    }

    public IActionResult Create()
    {
        return View(new FormEditDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FormEditDto model, string? templateId)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _formService.CreateFormAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Form oluşturulamadı.");
            return View(model);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction("Index", "FormBuilder", new { id = result.Data, template = templateId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var form = await _formService.GetFormDetailAsync(id);
        if (form == null)
        {
            return NotFound();
        }

        var model = MapToEditDto(form);

        ViewBag.PublicUrl = _formService.GetPublicUrl(form.Slug, $"{Request.Scheme}://{Request.Host}");
        ViewBag.IsPublished = form.IsPublished;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FormEditDto model, IFormFile? logoFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (logoFile != null && logoFile.Length > 0)
        {
            var logoUrl = await SaveLogoAsync(model.Id, logoFile);
            if (logoUrl != null)
            {
                model.LogoUrl = logoUrl;
            }
        }

        var result = await _formService.UpdateFormAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Form güncellenemedi.");
            return View(model);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Edit), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _formService.DeleteFormAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Copy(int id)
    {
        var result = await _formService.CopyFormAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var result = await _formService.ToggleActiveAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        var result = await _formService.PublishFormAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unpublish(int id)
    {
        var result = await _formService.UnpublishFormAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Edit), new { id });
    }

    public async Task<IActionResult> Preview(int id)
    {
        var form = await _formService.GetFormDetailAsync(id);
        if (form == null)
        {
            return NotFound();
        }

        return View(form);
    }

    private static FormEditDto MapToEditDto(FormDetailDto form) => new()
    {
        Id = form.Id,
        Title = form.Title,
        Description = form.Description,
        Slug = form.Slug,
        IsActive = form.IsActive,
        Theme = form.Theme,
        LogoUrl = form.LogoUrl,
        PrimaryColor = form.PrimaryColor,
        BackgroundColor = form.BackgroundColor,
        ButtonColor = form.ButtonColor,
        EmailNotificationEnabled = form.EmailNotificationEnabled,
        NotificationEmail = form.NotificationEmail
    };

    private async Task<string?> SaveLogoAsync(int formId, IFormFile file)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext) || file.Length > 2 * 1024 * 1024)
        {
            return null;
        }

        var dir = Path.Combine(_env.WebRootPath, "uploads", "logos");
        Directory.CreateDirectory(dir);
        var fileName = $"form_{formId}_{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(dir, fileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/logos/{fileName}";
    }
}
