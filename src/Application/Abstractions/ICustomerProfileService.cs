using Application.DTOs.Customers;

namespace Application.Abstractions;

public interface ICustomerProfileService
{
    Task<CustomerProfileDto> GetProfileAsync(long userId);
    Task<CustomerProfileDto> UpdateProfileAsync(UpdateCustomerProfileRequest request, long userId);
    Task<int> GetTotalOrdersAsync(long userId);
    Task<int> GetTotalWishlistItemsAsync(long userId);
    Task<DateTime?> GetLastLoginAsync(long userId);
}
