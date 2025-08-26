using Application.DTOs.Stores;
using Domain.Entities;

namespace Application.Abstractions;

public interface IStoreApplicationRepository
{
    Task<StoreApplication> AddAsync(StoreApplication application);
    Task<StoreApplication?> GetByIdAsync(long id);
    Task<IEnumerable<StoreApplication>> GetAllAsync();
    Task<StoreApplication> UpdateAsync(StoreApplication application);
    Task<bool> DeleteAsync(long id);
    Task<IEnumerable<StoreApplication>> GetByStatusAsync(string status);
    Task<IEnumerable<StoreApplication>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<StoreApplication>> SearchAsync(StoreApplicationSearchRequest request);
}
