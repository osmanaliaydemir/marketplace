using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Customers;
using Application.DTOs.Users;
using Application.Abstractions;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerProfileService _customerProfileService;
    private readonly ICustomerAddressService _customerAddressService;
    private readonly IPasswordService _passwordService;
    private readonly IOrderService _orderService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerProfileService customerProfileService,
        ICustomerAddressService customerAddressService,
        IPasswordService passwordService,
        IOrderService orderService,
        ILogger<CustomersController> logger)
    {
        _customerProfileService = customerProfileService;
        _customerAddressService = customerAddressService;
        _passwordService = passwordService;
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Müşteri profil bilgilerini getirir
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<CustomerProfileDto>> GetProfile()
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var profile = await _customerProfileService.GetProfileAsync(userId);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer profile for user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "Profil bilgileri alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Müşteri profil bilgilerini günceller
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<CustomerProfileDto>> UpdateProfile([FromBody] UpdateCustomerProfileRequest request)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var updatedProfile = await _customerProfileService.UpdateProfileAsync(request, userId);
            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer profile for user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "Profil güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Müşteri adreslerini getirir
    /// </summary>
    [HttpGet("addresses")]
    public async Task<ActionResult<List<CustomerAddressDto>>> GetAddresses()
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var addresses = await _customerAddressService.GetAddressesByUserIdAsync(userId);
            return Ok(addresses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer addresses for user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "Adres bilgileri alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Yeni adres ekler
    /// </summary>
    [HttpPost("addresses")]
    public async Task<ActionResult<CustomerAddressDto>> AddAddress([FromBody] CreateCustomerAddressRequest request)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var newAddress = await _customerAddressService.CreateAddressAsync(request, userId);
            return CreatedAtAction(nameof(GetAddresses), new { id = newAddress.Id }, newAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding customer address for user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "Adres eklenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Adres günceller
    /// </summary>
    [HttpPut("addresses/{id}")]
    public async Task<ActionResult<CustomerAddressDto>> UpdateAddress(long id, [FromBody] UpdateCustomerAddressRequest request)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var updatedAddress = await _customerAddressService.UpdateAddressAsync(request, userId);
            return Ok(updatedAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer address {AddressId} for user ID: {UserId}", id, userId);
            return StatusCode(500, new { Message = "Adres güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Adres siler
    /// </summary>
    [HttpDelete("addresses/{id}")]
    public async Task<ActionResult> DeleteAddress(long id)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var result = await _customerAddressService.DeleteAddressAsync(id, userId);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound(new { Message = "Adres bulunamadı" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer address {AddressId} for user ID: {UserId}", id, userId);
            return StatusCode(500, new { Message = "Adres silinirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Müşteri siparişlerini getirir
    /// </summary>
    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedResult<CustomerOrderDto>>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId, page, pageSize);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer orders for user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "Sipariş bilgileri alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Müşteri sipariş detayını getirir
    /// </summary>
    [HttpGet("orders/{id}")]
    public async Task<ActionResult<CustomerOrderDetailDto>> GetOrder(long id)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { Message = "Sipariş bulunamadı" });
            }
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer order {OrderId} for user ID: {UserId}", id, userId);
            return StatusCode(500, new { Message = "Sipariş detayı alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Müşteri favori ürünlerini getirir
    /// </summary>
    [HttpGet("wishlist")]
    public async Task<ActionResult<PaginatedResult<CustomerWishlistItemDto>>> GetWishlist([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var wishlistItems = await _orderService.GetWishlistItemsByUserIdAsync(userId, page, pageSize);
            return Ok(wishlistItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer wishlist for user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "Favori ürünler alınırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Şifre değiştirme
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] Application.DTOs.Customers.ChangePasswordRequest request)
    {
        long userId = 0;
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var result = await _passwordService.ChangePasswordAsync(request, userId);
            if (result)
            {
                return Ok(new { Message = "Şifre başarıyla değiştirildi" });
            }
            else
            {
                return BadRequest(new { Message = "Şifre değiştirilemedi" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user ID: {UserId}", userId);
            return StatusCode(500, new { Message = "Şifre değiştirilirken bir hata oluştu" });
        }
    }
}
