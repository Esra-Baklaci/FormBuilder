using DynamicFormBuilder.Entity.Enums;

namespace DynamicFormBuilder.Business.Models;

public class ConditionalLogicDto
{
    public int? Id { get; set; }
    public string SourceFieldClientId { get; set; } = string.Empty;
    public ConditionalOperator Operator { get; set; }
    public string Value { get; set; } = string.Empty;
    public string TargetFieldClientId { get; set; } = string.Empty;
    public ConditionalActionType ActionType { get; set; }
}
