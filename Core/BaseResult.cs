namespace JobOdysseyApi.Core;

public class Result
{
    public bool Succeeded { get; set; }
    public string Error { get; set; } = string.Empty;
    public int ErrorCode { get; set; }
 
    public static Result Success()
    {
        return new Result { Succeeded = true };
    }

    public static Result Failure(string error, int errorCode)
    {
        return new Result { Succeeded = false, Error = error, ErrorCode = errorCode };
    }
}