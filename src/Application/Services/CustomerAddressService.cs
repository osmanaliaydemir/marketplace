using Application.Abstractions;
using Application.DTOs.Customers;
using Application.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CustomerAddressService : ICustomerAddressService
{
    private readonly ICustomerAddressRepository _addressRepository;
    private readonly ILogger<CustomerAddressService> _logger;

    public CustomerAddressService(
        ICustomerAddressRepository addressRepository,
        ILogger<CustomerAddressService> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<List<CustomerAddressDto>> GetAddressesByUserIdAsync(long userId)
    {
        try
        {
            var addresses = await _addressRepository.GetAddressesByUserIdAsync(userId);
            
            return addresses.Select(a => new CustomerAddressDto
            {
                Id = a.Id,
                Title = a.Title,
                RecipientName = a.RecipientName,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Phone = a.Phone,
                IsDefault = a.IsDefault
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting addresses for user ID: {UserId}", userId);
            throw new RepositoryException("Adres bilgileri alınırken bir hata oluştu", "GetAddresses", "CustomerAddress", ex);
        }
    }

    public async Task<CustomerAddressDto?> GetAddressByIdAsync(long addressId, long userId)
    {
        try
        {
            var address = await _addressRepository.GetAddressByIdAsync(addressId, userId);
            
            if (address == null) return null;

            return new CustomerAddressDto
            {
                Id = address.Id,
                Title = address.Title,
                RecipientName = address.RecipientName,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Phone = address.Phone,
                IsDefault = address.IsDefault
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting address {AddressId} for user ID: {UserId}", addressId, userId);
            throw new RepositoryException("Adres bilgisi alınırken bir hata oluştu", "GetAddress", "CustomerAddress", ex);
        }
    }

    public async Task<CustomerAddressDto> CreateAddressAsync(CreateCustomerAddressRequest request, long userId)
    {
        try
        {
            // Eğer yeni adres varsayılan olarak ayarlanacaksa, diğer adresleri varsayılan olmaktan çıkar
            if (request.IsDefault)
            {
                await _addressRepository.ClearDefaultAddressesAsync(userId);
            }

            var address = new CustomerAddress
            {
                CustomerId = userId,
                Title = request.Title,
                RecipientName = request.RecipientName,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Phone = request.Phone,
                IsDefault = request.IsDefault,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdAddress = await _addressRepository.CreateAddressAsync(address);

            return new CustomerAddressDto
            {
                Id = createdAddress.Id,
                Title = createdAddress.Title,
                RecipientName = createdAddress.RecipientName,
                AddressLine1 = createdAddress.AddressLine1,
                AddressLine2 = createdAddress.AddressLine2,
                City = createdAddress.City,
                State = createdAddress.State,
                PostalCode = createdAddress.PostalCode,
                Phone = createdAddress.Phone,
                IsDefault = createdAddress.IsDefault
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address for user ID: {UserId}", userId);
            throw new RepositoryException("Adres oluşturulurken bir hata oluştu", "CreateAddress", "CustomerAddress", ex);
        }
    }

    public async Task<CustomerAddressDto> UpdateAddressAsync(UpdateCustomerAddressRequest request, long userId)
    {
        try
        {
            var address = await _addressRepository.GetAddressByIdAsync(request.Id, userId);
            if (address == null)
            {
                throw new Exception($"Adres bulunamadı: {request.Id}");
            }

            // Eğer adres varsayılan olarak ayarlanacaksa, diğer adresleri varsayılan olmaktan çıkar
            if (request.IsDefault && !address.IsDefault)
            {
                await _addressRepository.ClearDefaultAddressesAsync(userId);
            }

            address.Title = request.Title;
            address.RecipientName = request.RecipientName;
            address.AddressLine1 = request.AddressLine1;
            address.AddressLine2 = request.AddressLine2;
            address.City = request.City;
            address.State = request.State;
            address.PostalCode = request.PostalCode;
            address.Phone = request.Phone;
            address.IsDefault = request.IsDefault;
            address.ModifiedAt = DateTime.UtcNow;

            var updatedAddress = await _addressRepository.UpdateAddressAsync(address);

            return new CustomerAddressDto
            {
                Id = updatedAddress.Id,
                Title = updatedAddress.Title,
                RecipientName = updatedAddress.RecipientName,
                AddressLine1 = updatedAddress.AddressLine1,
                AddressLine2 = updatedAddress.AddressLine2,
                City = updatedAddress.City,
                State = updatedAddress.State,
                PostalCode = updatedAddress.PostalCode,
                Phone = updatedAddress.Phone,
                IsDefault = updatedAddress.IsDefault
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address {AddressId} for user ID: {UserId}", request.Id, userId);
            throw new RepositoryException("Adres güncellenirken bir hata oluştu", "UpdateAddress", "CustomerAddress", ex);
        }
    }

    public async Task<bool> DeleteAddressAsync(long addressId, long userId)
    {
        try
        {
            var address = await _addressRepository.GetAddressByIdAsync(addressId, userId);
            if (address == null)
            {
                return false;
            }

            // Eğer silinen adres varsayılan adres ise, ilk adresi varsayılan yap
            if (address.IsDefault)
            {
                var addresses = await _addressRepository.GetAddressesByUserIdAsync(userId);
                var firstAddress = addresses.FirstOrDefault(a => a.Id != addressId);
                
                if (firstAddress != null)
                {
                    firstAddress.IsDefault = true;
                    firstAddress.ModifiedAt = DateTime.UtcNow;
                    await _addressRepository.UpdateAddressAsync(firstAddress);
                }
            }

            return await _addressRepository.DeleteAddressAsync(addressId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address {AddressId} for user ID: {UserId}", addressId, userId);
            throw new RepositoryException("Adres silinirken bir hata oluştu", "DeleteAddress", "CustomerAddress", ex);
        }
    }

    public async Task<bool> SetDefaultAddressAsync(long addressId, long userId)
    {
        try
        {
            var address = await _addressRepository.GetAddressByIdAsync(addressId, userId);
            if (address == null)
            {
                return false;
            }

            if (address.IsDefault)
            {
                return true; // Zaten varsayılan
            }

            // Diğer tüm adresleri varsayılan olmaktan çıkar
            await _addressRepository.ClearDefaultAddressesAsync(userId);

            // Bu adresi varsayılan yap
            address.IsDefault = true;
            address.ModifiedAt = DateTime.UtcNow;

            await _addressRepository.UpdateAddressAsync(address);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting default address {AddressId} for user ID: {UserId}", addressId, userId);
            throw new RepositoryException("Varsayılan adres ayarlanırken bir hata oluştu", "SetDefaultAddress", "CustomerAddress", ex);
        }
    }
}
