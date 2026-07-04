namespace DynamicFormBuilder.Business.Models;

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int? Id { get; set; }

    public static ServiceResult Ok(string? message = null, int? id = null) =>
        new() { Success = true, Message = message, Id = id };

    public static ServiceResult Fail(string message) =>
        new() { Success = false, Message = message };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public new static ServiceResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
