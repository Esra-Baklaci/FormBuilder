namespace DynamicFormBuilder.Business.Models;

public class DashboardDto
{
    public int TotalForms { get; set; }
    public int ActiveForms { get; set; }
    public int TotalResponses { get; set; }
    public List<FormListItemDto> RecentForms { get; set; } = new();
    public List<ResponseListItemDto> RecentResponses { get; set; } = new();
}
