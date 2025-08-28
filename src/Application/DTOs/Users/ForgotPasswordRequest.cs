using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Users;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "E-posta adresi gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; init; } = string.Empty;
}
