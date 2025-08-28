using Application.DTOs.Stores;

namespace Application.Abstractions;

public interface IStoreService
{
    // CRUD Operations
    Task<StoreDetailDto> CreateAsync(StoreCreateRequest request);
    Task<StoreDetailDto> UpdateAsync(long id, StoreUpdateRequest request);
    Task<bool> DeleteAsync(long id);
    Task<StoreDetailDto?> GetByIdAsync(long id);
    Task<StoreDetailDto?> GetBySlugAsync(string slug);
    
    // Listing and Search
    Task<StoreListResponse> ListAsync(StoreListRequest request);
    Task<StoreListResponse> SearchAsync(StoreSearchRequest request);
    Task<StoreListResponse> GetBySellerAsync(long sellerId, StoreListRequest request);
    
    // Status Management
    Task<bool> SetActiveAsync(long id, bool isActive);
    Task<bool> ApproveAsync(long id);
    Task<bool> RejectAsync(long id, string reason);
    
    // Store Management
    Task<StoreDetailDto?> GetByCurrentSellerAsync(long userId);
    Task<StoreStatsDto> GetStatsAsync();
    Task<IEnumerable<StoreDto>> GetActiveStoresAsync();
    Task<IEnumerable<StoreDto>> GetPendingApprovalAsync();
}
