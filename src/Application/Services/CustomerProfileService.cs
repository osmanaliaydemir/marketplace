using Application.Abstractions;
using Application.DTOs.Customers;
using Application.DTOs.Users;
using Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CustomerProfileService : ICustomerProfileService
{
    private readonly IAppUserService _appUserService;
    private readonly IOrderService _orderService;
    private readonly ILogger<CustomerProfileService> _logger;

    public CustomerProfileService(
        IAppUserService appUserService,
        IOrderService orderService,
        ILogger<CustomerProfileService> logger)
    {
        _appUserService = appUserService;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<CustomerProfileDto> GetProfileAsync(long userId)
    {
        try
        {
            var user = await _appUserService.GetByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("AppUser", userId, "Kullanıcı bulunamadı");
            }

            var totalOrders = await GetTotalOrdersAsync(userId);
            var totalWishlist = await GetTotalWishlistItemsAsync(userId);
            var lastLogin = await GetLastLoginAsync(userId);

            var profile = new CustomerProfileDto
            {
                Id = user.Id,
                FirstName = user.FullName.Split(' ').FirstOrDefault() ?? "",
                LastName = user.FullName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                Email = user.Email,
                Phone = null, // User entity'de yok, ayrı bir tablo gerekli
                BirthDate = null, // User entity'de yok, ayrı bir tablo gerekli
                Gender = null, // User entity'de yok, ayrı bir tablo gerekli
                MemberSince = user.CreatedAt,
                TotalOrders = totalOrders,
                TotalWishlist = totalWishlist,
                LastLogin = lastLogin ?? user.CreatedAt
            };

            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer profile for user ID: {UserId}", userId);
            throw new RepositoryException("Profil bilgileri alınırken bir hata oluştu", "GetProfile", "CustomerProfile", ex);
        }
    }

    public async Task<CustomerProfileDto> UpdateProfileAsync(UpdateCustomerProfileRequest request, long userId)
    {
        try
        {
            var user = await _appUserService.GetByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("AppUser", userId, "Kullanıcı bulunamadı");
            }

            var updateRequest = new UpdateUserRequest
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{request.FirstName} {request.LastName}".Trim(),
                Role = user.Role,
                IsActive = user.IsActive
            };

            await _appUserService.UpdateAsync(updateRequest);

            // Güncellenmiş profili döndür
            return await GetProfileAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer profile for user ID: {UserId}", userId);
            throw new RepositoryException("Profil güncellenirken bir hata oluştu", "UpdateProfile", "CustomerProfile", ex);
        }
    }

    public async Task<int> GetTotalOrdersAsync(long userId)
    {
        try
        {
            // OrderService üzerinden sipariş sayısını al
            // TODO: OrderService'e GetOrderCountByUserIdAsync metodu eklenmeli
            await Task.CompletedTask; // Geçici olarak Task.CompletedTask kullan
            return 0; // Geçici olarak 0 döndür
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total orders for user ID: {UserId}", userId);
            return 0;
        }
    }

    public async Task<int> GetTotalWishlistItemsAsync(long userId)
    {
        try
        {
            // Wishlist tablosu yoksa 0 döndür
            // TODO: Wishlist tablosu oluşturulduğunda bu kısım güncellenecek
            await Task.CompletedTask; // Geçici olarak Task.CompletedTask kullan
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total wishlist items for user ID: {UserId}", userId);
            return 0;
        }
    }

    public async Task<DateTime?> GetLastLoginAsync(long userId)
    {
        try
        {
            // UserLoginHistory tablosu yoksa null döndür
            // TODO: UserLoginHistory tablosu oluşturulduğunda bu kısım güncellenecek
            await Task.CompletedTask; // Geçici olarak Task.CompletedTask kullan
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting last login for user ID: {UserId}", userId);
            return null;
        }
    }
}
