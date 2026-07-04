namespace DynamicFormBuilder.Business.Models;

public class ResponseListItemDto
{
    public int Id { get; set; }
    public int FormId { get; set; }
    public string FormTitle { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string? IpAddress { get; set; }
    public int ValueCount { get; set; }
}
