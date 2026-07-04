using System.Text.Json;
using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.WebUI.Controllers;

public class FormBuilderController : Controller
{
    private readonly IFormBuilderService _formBuilderService;
    private readonly IFormService _formService;

    public FormBuilderController(IFormBuilderService formBuilderService, IFormService formService)
    {
        _formBuilderService = formBuilderService;
        _formService = formService;
    }

    public async Task<IActionResult> Index(int id)
    {
        var form = await _formBuilderService.GetBuilderDataAsync(id);
        if (form == null)
        {
            return NotFound();
        }

        return View(form);
    }

    [HttpGet]
    public async Task<IActionResult> GetData(int id)
    {
        var form = await _formBuilderService.GetBuilderDataAsync(id);
        if (form == null)
        {
            return NotFound();
        }

        return Json(form);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromBody] FormBuilderSaveDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { success = false, message = "Geçersiz veri." });
        }

        var result = await _formBuilderService.SaveBuilderAsync(dto);
        return Json(new { success = result.Success, message = result.Message });
    }
}
