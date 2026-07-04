using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Entity.Entities;

public class ConditionalLogic : BaseEntity
{
    public int FormId { get; set; }
    public string SourceFieldClientId { get; set; } = string.Empty;
    public int? SourceFieldId { get; set; }
    public ConditionalOperator Operator { get; set; }
    public string Value { get; set; } = string.Empty;
    public string TargetFieldClientId { get; set; } = string.Empty;
    public int? TargetFieldId { get; set; }
    public ConditionalActionType ActionType { get; set; }

    public DynamicForm Form { get; set; } = null!;
}
