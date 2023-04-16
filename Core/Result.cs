namespace JobOdysseyApi.Core;
public class Result<T>
{
    public bool Succeeded { get; set; }
    public string Error { get; set; } = string.Empty;
    public int ErrorCode { get; set; }
    public T? Data { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static Result<T> Failure(string error, int errorCode)
    {
        return new Result<T> { Succeeded = false, Error = error, ErrorCode = errorCode };
    }
}