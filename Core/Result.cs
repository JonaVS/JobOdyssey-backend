namespace JobOdysseyApi.Core;

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public new static Result<T> Failure(string error, int errorCode)
    {
        return new Result<T> { Succeeded = false, Error = error, ErrorCode = errorCode };
    }
}