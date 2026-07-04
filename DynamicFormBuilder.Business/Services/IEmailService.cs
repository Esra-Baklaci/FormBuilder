namespace DynamicFormBuilder.Business.Services;

public interface IEmailService
{
    Task SendFormSubmissionNotificationAsync(string toEmail, string formTitle, DateTime submittedAt, IReadOnlyList<(string Label, string Value)> values);
}
