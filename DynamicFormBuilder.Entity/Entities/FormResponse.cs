namespace DynamicFormBuilder.Entity.Entities;

public class FormResponse : BaseEntity
{
    public int FormId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }

    public DynamicForm Form { get; set; } = null!;
    public ICollection<FormResponseValue> Values { get; set; } = new List<FormResponseValue>();
}
