namespace Application.Abstractions;

public interface IResult
{
    bool Success { get; }
    string? Error { get; }
}

public sealed record Result(bool Success, string? Error = null) : IResult
{
    public static Result Ok() => new(true);
    public static Result Fail(string error) => new(false, error);
}
