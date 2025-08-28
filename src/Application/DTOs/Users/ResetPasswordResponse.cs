namespace Application.DTOs.Users;

public class ResetPasswordResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Email { get; init; }
}
