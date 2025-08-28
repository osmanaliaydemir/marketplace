using Application.DTOs.Customers;

namespace Application.Abstractions;

public interface ICustomerAddressService
{
    Task<List<CustomerAddressDto>> GetAddressesByUserIdAsync(long userId);
    Task<CustomerAddressDto?> GetAddressByIdAsync(long addressId, long userId);
    Task<CustomerAddressDto> CreateAddressAsync(CreateCustomerAddressRequest request, long userId);
    Task<CustomerAddressDto> UpdateAddressAsync(UpdateCustomerAddressRequest request, long userId);
    Task<bool> DeleteAddressAsync(long addressId, long userId);
    Task<bool> SetDefaultAddressAsync(long addressId, long userId);
}
