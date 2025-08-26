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
	
	// New methods for StoreApplicationsController
	Task<StoreApplicationDetailDto> CreateApplicationAsync(StoreApplicationCreateRequest request);
	Task<IEnumerable<StoreApplicationListDto>> GetApplicationsAsync(int page = 1, int pageSize = 20);
	Task<StoreApplicationDetailDto> GetApplicationByIdAsync(long id);
	Task<StoreApplicationDetailDto> UpdateApplicationAsync(long id, StoreApplicationUpdateRequest request);
	Task<StoreApplicationDetailDto> ApproveApplicationAsync(long id, StoreApplicationApprovalRequest request);
	Task<StoreApplicationDetailDto> RejectApplicationAsync(long id, StoreApplicationRejectionRequest request);
	Task<StoreApplicationDetailDto> DeleteApplicationAsync(long id);
}
