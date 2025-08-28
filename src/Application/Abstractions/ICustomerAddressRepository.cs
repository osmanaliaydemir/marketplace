using Application.DTOs.Customers;
using Domain.Entities;

namespace Application.Abstractions;

public interface ICustomerAddressRepository
{
    Task<List<CustomerAddress>> GetAddressesByUserIdAsync(long userId);
    Task<CustomerAddress?> GetAddressByIdAsync(long addressId, long userId);
    Task<CustomerAddress> CreateAddressAsync(CustomerAddress address);
    Task<CustomerAddress> UpdateAddressAsync(CustomerAddress address);
    Task<bool> DeleteAddressAsync(long addressId, long userId);
    Task<List<CustomerAddress>> GetDefaultAddressesByUserIdAsync(long userId);
    Task ClearDefaultAddressesAsync(long userId);
}
