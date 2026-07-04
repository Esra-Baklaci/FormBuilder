using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicFormBuilder.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        services.AddScoped<IFormService, FormService>();
        services.AddScoped<IFormBuilderService, FormBuilderService>();
        services.AddScoped<IPublicFormService, PublicFormService>();
        services.AddScoped<IResponseService, ResponseService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPdfExportService, PdfExportService>();

        return services;
    }
}
