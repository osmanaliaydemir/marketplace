using Domain.Entities;

namespace Application.Abstractions;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneAsync(string phone);
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
    Task<bool> IsEmailUniqueAsync(string email, long? excludeId = null);
    Task<bool> IsPhoneUniqueAsync(string phone, long? excludeId = null);
    Task<int> GetTotalCustomerCountAsync();
    Task<decimal> GetTotalCustomerSpendingAsync(long customerId);
}
