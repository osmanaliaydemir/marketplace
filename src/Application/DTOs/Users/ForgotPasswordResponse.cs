namespace Application.DTOs.Users;

public class ForgotPasswordResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Email { get; init; }
}
