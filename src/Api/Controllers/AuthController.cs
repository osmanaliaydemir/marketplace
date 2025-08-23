using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Persistence.Repositories;
using Domain.Entities;
using System.Security.Cryptography;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IStoreUnitOfWork _unitOfWork;

    public AuthController(IConfiguration configuration, IStoreUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.EmailOrUsername) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Email/kullanıcı adı ve şifre gereklidir" });
            }

            // Kullanıcıyı bul (email veya username ile)
            var user = await FindUserByEmailOrUsername(request.EmailOrUsername);
            if (user == null)
            {
                return Unauthorized(new { Message = "Geçersiz email/kullanıcı adı veya şifre" });
            }

            // Şifreyi doğrula
            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Geçersiz email/kullanıcı adı veya şifre" });
            }

            // Kullanıcı aktif mi kontrol et
            if (!user.IsActive)
            {
                return Unauthorized(new { Message = "Hesabınız aktif değil. Lütfen yönetici ile iletişime geçin." });
            }

            // JWT token oluştur
            var token = GenerateJwtToken(user);

            // Refresh token oluştur (basit UUID, production'da Redis'te saklanmalı)
            var refreshToken = Guid.NewGuid().ToString();

            return Ok(new LoginResponse
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                Role = user.Role.ToString(),
                UserName = user.FullName,
                UserId = user.Id,
                Message = "Giriş başarılı"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Giriş yapılırken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { Message = "Refresh token gereklidir" });
            }

            // TODO: Refresh token'ı Redis'ten doğrula
            // Şimdilik basit bir kontrol yapıyoruz
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return Unauthorized(new { Message = "Geçersiz refresh token" });
            }

            // JWT token'dan kullanıcı bilgilerini çıkar
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!);
            
            try
            {
                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = false // Expired token'ları da kabul et
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = long.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                // Kullanıcıyı bul
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return Unauthorized(new { Message = "Kullanıcı bulunamadı veya aktif değil" });
                }

                // Yeni token oluştur
                var newToken = GenerateJwtToken(user);
                var newRefreshToken = Guid.NewGuid().ToString();

                return Ok(new LoginResponse
                {
                    Success = true,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                                    Role = user.Role.ToString(),
                UserName = user.FullName,
                UserId = user.Id,
                Message = "Token yenilendi"
                });
            }
            catch
            {
                return Unauthorized(new { Message = "Geçersiz token" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Token yenilenirken bir hata oluştu", Error = ex.Message });
        }
    }

    // Development için geçici endpoint (production'da kaldırılacak)
    [HttpPost("dev-login")]
    public IActionResult DevLogin([FromBody] DevLoginRequest request)
    {
        // Bu endpoint sadece development ortamında kullanılmalı
        if (!_configuration.GetValue<bool>("AllowDevLogin", false))
        {
            return NotFound();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, request.UserId.ToString()),
            new Claim(ClaimTypes.Role, request.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    private async Task<AppUser?> FindUserByEmailOrUsername(string emailOrUsername)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return users.FirstOrDefault(u => 
            u.Email.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase) ||
            u.FullName.Equals(emailOrUsername, StringComparison.OrdinalIgnoreCase));
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // Basit hash kontrolü (production'da BCrypt/Argon2 kullanılmalı)
        var hashedPassword = HashPassword(password);
        return passwordHash.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private string GenerateJwtToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(24); // 24 saat geçerli

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public sealed class LoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public sealed class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public sealed class DevLoginRequest
{
    public long UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}


