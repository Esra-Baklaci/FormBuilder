namespace DynamicFormBuilder.Business.Models;

public class ResponseDetailDto
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public string FormTitle { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string? IpAddress { get; set; }
    public List<ResponseValueDto> Values { get; set; } = new();
}
