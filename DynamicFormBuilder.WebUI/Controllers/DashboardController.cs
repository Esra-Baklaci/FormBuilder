using DynamicFormBuilder.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.WebUI.Controllers;

public class DashboardController : Controller
{
    private readonly IFormService _formService;

    public DashboardController(IFormService formService)
    {
        _formService = formService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _formService.GetDashboardAsync();
        return View(model);
    }
}
