using DynamicFormBuilder.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.WebUI.Controllers;

public class ResponseController : Controller
{
    private readonly IResponseService _responseService;
    private readonly IFormService _formService;
    private readonly IPdfExportService _pdfExportService;

    public ResponseController(
        IResponseService responseService,
        IFormService formService,
        IPdfExportService pdfExportService)
    {
        _responseService = responseService;
        _formService = formService;
        _pdfExportService = pdfExportService;
    }

    public async Task<IActionResult> Index(int formId)
    {
        var form = await _formService.GetFormDetailAsync(formId);
        if (form == null)
        {
            return NotFound();
        }

        var responses = await _responseService.GetResponsesByFormIdAsync(formId);
        ViewBag.Form = form;
        return View(responses);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var response = await _responseService.GetResponseDetailAsync(id);
        if (response == null)
        {
            return NotFound();
        }

        return View(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int formId)
    {
        var result = await _responseService.DeleteResponseAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(Index), new { formId });
    }

    public async Task<IActionResult> ExportExcel(int formId)
    {
        var form = await _formService.GetFormDetailAsync(formId);
        if (form == null)
        {
            return NotFound();
        }

        var bytes = await _responseService.ExportToExcelAsync(formId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{form.Slug}-responses.xlsx");
    }

    public async Task<IActionResult> ExportPdf(int id)
    {
        var response = await _responseService.GetResponseDetailAsync(id);
        if (response == null)
        {
            return NotFound();
        }

        var bytes = await _pdfExportService.ExportResponseToPdfAsync(response);
        return File(bytes, "application/octet-stream", $"response-{id}.txt");
    }
}
