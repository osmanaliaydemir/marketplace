using Application.DTOs.Stores;
using Domain.Entities;

namespace Application.Abstractions;

public interface IStoreApplicationService
{
	Task<StoreApplication> SubmitAsync(StoreApplication application);
	Task<bool> ApproveAsync(long applicationId, long approvedByUserId);
	Task<bool> RejectAsync(long applicationId, string reason, long rejectedByUserId);
	Task<StoreApplication?> GetAsync(long id);
	Task<IEnumerable<StoreApplication>> ListPendingAsync();
	
	// Dashboard functionality
	Task<StoreApplicationSearchResponse> SearchAsync(StoreApplicationSearchRequest request);
	Task<StoreApplicationStatsDto> GetStatsAsync();
	Task<IEnumerable<StoreApplication>> GetByStatusAsync(string status);
	Task<IEnumerable<StoreApplication>> GetByDateRangeAsync(DateTime from, DateTime to);
}
