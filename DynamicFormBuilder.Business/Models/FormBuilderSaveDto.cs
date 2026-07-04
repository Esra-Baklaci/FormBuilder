using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Models;

public class FormBuilderSaveDto
{
    public int FormId { get; set; }
    public List<FormFieldDto> Fields { get; set; } = new();
    public List<ConditionalLogicDto> ConditionalLogics { get; set; } = new();
}
