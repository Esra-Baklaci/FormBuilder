namespace DynamicFormBuilder.Business.Models;

public class FileUploadItem
{
    public Stream Stream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
}
