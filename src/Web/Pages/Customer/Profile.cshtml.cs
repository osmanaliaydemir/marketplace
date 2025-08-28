using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Application.DTOs.Customers;
using Application.DTOs.Users;
using Application.Abstractions;

namespace Web.Pages.Customer;

[Authorize(Roles = "Customer")]
public sealed class ProfileModel : PageModel
{
    private readonly ICustomerProfileService _customerProfileService;
    private readonly ICustomerAddressService _customerAddressService;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(
        ICustomerProfileService customerProfileService,
        ICustomerAddressService customerAddressService,
        IPasswordService passwordService,
        ILogger<ProfileModel> logger)
    {
        _customerProfileService = customerProfileService;
        _customerAddressService = customerAddressService;
        _passwordService = passwordService;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    [HttpPost]
    public async Task<IActionResult> OnPostGetProfileAsync()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var profile = await _customerProfileService.GetProfileAsync(userId);
            return new JsonResult(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer profile for user ID: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Profil bilgileri alınırken bir hata oluştu" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> OnPostGetAddressesAsync()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var addresses = await _customerAddressService.GetAddressesByUserIdAsync(userId);
            return new JsonResult(addresses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer addresses for user ID: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Adres bilgileri alınırken bir hata oluştu" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> OnPostUpdateProfileAsync([FromBody] UpdateCustomerProfileRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            await _customerProfileService.UpdateProfileAsync(request, userId);
            return new JsonResult(new { Message = "Profil başarıyla güncellendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer profile for user ID: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Profil güncellenirken bir hata oluştu" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> OnPostSaveAddressAsync([FromBody] CreateCustomerAddressRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var newAddress = await _customerAddressService.CreateAddressAsync(request, userId);
            return new JsonResult(new { Message = "Adres başarıyla kaydedildi", Address = newAddress });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving customer address for user ID: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Adres kaydedilirken bir hata oluştu" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> OnPostUpdateAddressAsync([FromBody] UpdateCustomerAddressRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var updatedAddress = await _customerAddressService.UpdateAddressAsync(request, userId);
            return new JsonResult(new { Message = "Adres başarıyla güncellendi", Address = updatedAddress });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer address for user ID: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Adres güncellenirken bir hata oluştu" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> OnPostDeleteAddressAsync(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _customerAddressService.DeleteAddressAsync(id, userId);
            if (result)
            {
                return new JsonResult(new { Message = "Adres başarıyla silindi" });
            }
            else
            {
                return NotFound(new { Message = "Adres bulunamadı" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer address {AddressId} for user ID: {UserId}", id, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Adres silinirken bir hata oluştu" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> OnPostChangePasswordAsync([FromBody] Application.DTOs.Customers.ChangePasswordRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _passwordService.ChangePasswordAsync(request, userId);
            if (result)
            {
                return new JsonResult(new { Message = "Şifre başarıyla değiştirildi" });
            }
            else
            {
                return BadRequest(new { Message = "Şifre değiştirilemedi" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user ID: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { Message = "Şifre değiştirilirken bir hata oluştu" });
        }
    }
}
