using ClosedXML.Excel;
using DynamicFormBuilder.Business.Models;
using DynamicFormBuilder.DataAccess.Repositories;
using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Services;

public class ResponseService : IResponseService
{
    private readonly IResponseRepository _responseRepository;
    private readonly IFormRepository _formRepository;

    public ResponseService(IResponseRepository responseRepository, IFormRepository formRepository)
    {
        _responseRepository = responseRepository;
        _formRepository = formRepository;
    }

    public async Task<IReadOnlyList<ResponseListItemDto>> GetResponsesByFormIdAsync(int formId)
    {
        var responses = await _responseRepository.GetByFormIdAsync(formId);
        return responses.Select(r => new ResponseListItemDto
        {
            Id = r.Id,
            FormId = r.FormId,
            FormTitle = string.Empty,
            SubmittedAt = r.SubmittedAt,
            IpAddress = r.IpAddress,
            ValueCount = r.Values.Count
        }).ToList();
    }

    public async Task<ResponseDetailDto?> GetResponseDetailAsync(int id)
    {
        var response = await _responseRepository.GetByIdWithValuesAsync(id);
        if (response == null)
        {
            return null;
        }

        return new ResponseDetailDto
        {
            Id = response.Id,
            FormId = response.FormId,
            FormTitle = response.Form?.Title ?? string.Empty,
            SubmittedAt = response.SubmittedAt,
            IpAddress = response.IpAddress,
            Values = response.Values
                .OrderBy(v => v.Field?.SortOrder ?? 0)
                .Select(v => new ResponseValueDto
                {
                    FieldId = v.FieldId,
                    FieldLabel = v.FieldLabel,
                    FieldType = v.FieldType,
                    Value = v.Value
                }).ToList()
        };
    }

    public async Task<ServiceResult> DeleteResponseAsync(int id)
    {
        var response = await _responseRepository.GetByIdAsync(id);
        if (response == null)
        {
            return ServiceResult.Fail("Yanıt bulunamadı.");
        }

        _responseRepository.Remove(response);
        await _responseRepository.SaveChangesAsync();

        return ServiceResult.Ok("Yanıt silindi.");
    }

    public async Task<byte[]> ExportToExcelAsync(int formId)
    {
        var form = await _formRepository.GetByIdWithDetailsAsync(formId)
            ?? throw new InvalidOperationException("Form bulunamadı.");

        var responses = await _responseRepository.GetByFormIdAsync(formId);
        var inputFields = form.Fields
            .Where(f => !f.IsHidden && f.FieldType != FieldType.Divider && f.FieldType != FieldType.Spacer)
            .OrderBy(f => f.SortOrder)
            .ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Responses");

        worksheet.Cell(1, 1).Value = "Yanıt ID";
        worksheet.Cell(1, 2).Value = "Gönderim Tarihi";
        worksheet.Cell(1, 3).Value = "IP Adresi";

        for (var i = 0; i < inputFields.Count; i++)
        {
            worksheet.Cell(1, i + 4).Value = inputFields[i].Label;
        }

        var row = 2;
        foreach (var response in responses)
        {
            worksheet.Cell(row, 1).Value = response.Id;
            worksheet.Cell(row, 2).Value = response.SubmittedAt.ToLocalTime();
            worksheet.Cell(row, 3).Value = response.IpAddress ?? string.Empty;

            for (var i = 0; i < inputFields.Count; i++)
            {
                var value = response.Values.FirstOrDefault(v => v.FieldId == inputFields[i].Id)?.Value ?? string.Empty;
                worksheet.Cell(row, i + 4).Value = value;
            }

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
