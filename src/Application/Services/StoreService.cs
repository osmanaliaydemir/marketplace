using Application.Abstractions;
using Application.DTOs.Stores;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public sealed class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly IAppUserRepository _userRepository;
    private readonly IAppUserService _appUserService;
    private readonly ILogger<StoreService> _logger;

    public StoreService(
        IStoreRepository storeRepository,
        ISellerRepository sellerRepository,
        IAppUserRepository userRepository,
        IAppUserService appUserService,
        ILogger<StoreService> logger)
    {
        _storeRepository = storeRepository;
        _sellerRepository = sellerRepository;
        _userRepository = userRepository;
        _appUserService = appUserService;
        _logger = logger;
    }

    public async Task<StoreDetailDto> CreateAsync(StoreCreateRequest request)
    {
        try
        {
            var store = new Store
            {
                SellerId = request.SellerId,
                Name = request.Name,
                Slug = GenerateSlug(request.Name),
                LogoUrl = request.LogoUrl,
                BannerUrl = request.BannerUrl,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            store = await _storeRepository.AddAsync(store);

            return await GetByIdAsync(store.Id) ?? throw new InvalidOperationException("Store created but could not retrieve");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating store: {StoreName}", request.Name);
            throw;
        }
    }

    public async Task<StoreDetailDto> UpdateAsync(long id, StoreUpdateRequest request)
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var store = stores.FirstOrDefault(s => s.Id == id);
            if (store == null)
                throw new InvalidOperationException($"Store with ID {id} not found");

            store.Name = request.Name;
            store.Slug = GenerateSlug(request.Name);
            store.LogoUrl = request.LogoUrl;
            store.BannerUrl = request.BannerUrl;
            store.Description = request.Description;
            store.IsActive = request.IsActive;
            store.ModifiedAt = DateTime.UtcNow;

            await _storeRepository.UpdateAsync(store);

            return await GetByIdAsync(id) ?? throw new InvalidOperationException("Store updated but could not retrieve");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating store ID: {StoreId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            await _storeRepository.DeleteAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting store ID: {StoreId}", id);
            return false;
        }
    }

    public async Task<StoreDetailDto?> GetByIdAsync(long id)
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var store = stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return null;

            var sellers = await _sellerRepository.GetAllAsync();
            var seller = sellers.FirstOrDefault(s => s.Id == store.SellerId);
            var users = seller != null ? await _userRepository.GetAllAsync() : null;
            var user = users?.FirstOrDefault(u => u.Id == seller?.UserId);

            return new StoreDetailDto
            {
                Id = store.Id,
                SellerId = store.SellerId,
                Name = store.Name,
                Slug = store.Slug,
                LogoUrl = store.LogoUrl,
                BannerUrl = store.BannerUrl,
                Description = store.Description,
                IsActive = store.IsActive,
                CreatedAt = store.CreatedAt,
                ModifiedAt = store.ModifiedAt,
                Seller = seller != null && user != null ? new SellerInfoDto
                {
                    Id = seller.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    CommissionRate = seller.CommissionRate,
                    IsActive = seller.IsActive
                } : null!,
                Categories = new List<Application.DTOs.Categories.StoreCategoryListDto>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store by ID: {StoreId}", id);
            return null;
        }
    }

    public async Task<StoreDetailDto?> GetBySlugAsync(string slug)
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var store = stores.FirstOrDefault(s => s.Slug == slug);
            return store != null ? await GetByIdAsync(store.Id) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store by slug: {Slug}", slug);
            return null;
        }
    }

    public async Task<StoreListResponse> ListAsync(StoreListRequest request)
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var sellers = await _sellerRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            var filteredStores = stores.AsQueryable();

            if (request.IsActive.HasValue)
                filteredStores = filteredStores.Where(s => s.IsActive == request.IsActive.Value);

            if (request.SellerId.HasValue)
                filteredStores = filteredStores.Where(s => s.SellerId == request.SellerId.Value);

            var totalCount = filteredStores.Count();
            var pagedStores = filteredStores
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = pagedStores.Select(store =>
            {
                var seller = sellers.FirstOrDefault(s => s.Id == store.SellerId);
                var user = users.FirstOrDefault(u => u.Id == seller?.UserId);

                return new StoreListDto
                {
                    Id = store.Id,
                    Name = store.Name,
                    Slug = store.Slug,
                    LogoUrl = store.LogoUrl,
                    IsActive = store.IsActive,
                    SellerName = user?.FullName ?? "Bilinmeyen Satıcı",
                    ProductCount = 0, // TODO: Calculate from products
                    CreatedAt = store.CreatedAt,
                    Seller = seller != null && user != null ? new SellerInfoDto
                    {
                        Id = seller.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        CommissionRate = seller.CommissionRate,
                        IsActive = seller.IsActive
                    } : null
                };
            });

            return new StoreListResponse
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing stores");
            return new StoreListResponse { Items = Enumerable.Empty<StoreListDto>() };
        }
    }

    public async Task<StoreListResponse> SearchAsync(StoreSearchRequest request)
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var sellers = await _sellerRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            var filteredStores = stores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filteredStores = filteredStores.Where(s => 
                    s.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    s.Slug.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (request.IsActive.HasValue)
                filteredStores = filteredStores.Where(s => s.IsActive == request.IsActive.Value);

            if (request.SellerId.HasValue)
                filteredStores = filteredStores.Where(s => s.SellerId == request.SellerId.Value);

            var totalCount = filteredStores.Count();
            var pagedStores = filteredStores
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = pagedStores.Select(store =>
            {
                var seller = sellers.FirstOrDefault(s => s.Id == store.SellerId);
                var user = users.FirstOrDefault(u => u.Id == seller?.UserId);

                return new StoreListDto
                {
                    Id = store.Id,
                    Name = store.Name,
                    Slug = store.Slug,
                    LogoUrl = store.LogoUrl,
                    IsActive = store.IsActive,
                    SellerName = user?.FullName ?? "Bilinmeyen Satıcı",
                    ProductCount = 0, // TODO: Calculate from products
                    CreatedAt = store.CreatedAt,
                    Seller = seller != null && user != null ? new SellerInfoDto
                    {
                        Id = seller.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        CommissionRate = seller.CommissionRate,
                        IsActive = seller.IsActive
                    } : null
                };
            });

            return new StoreListResponse
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching stores");
            return new StoreListResponse { Items = Enumerable.Empty<StoreListDto>() };
        }
    }

    public async Task<StoreListResponse> GetBySellerAsync(long sellerId, StoreListRequest request)
    {
        var modifiedRequest = new StoreListRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SellerId = sellerId
        };
        return await ListAsync(modifiedRequest);
    }

    public async Task<bool> SetActiveAsync(long id, bool isActive)
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var store = stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return false;

            store.IsActive = isActive;
            store.ModifiedAt = DateTime.UtcNow;

            await _storeRepository.UpdateAsync(store);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting store active status ID: {StoreId}", id);
            return false;
        }
    }

    public async Task<bool> ApproveAsync(long id)
    {
        return await SetActiveAsync(id, true);
    }

    public async Task<bool> RejectAsync(long id, string reason)
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var store = stores.FirstOrDefault(s => s.Id == id);
            if (store == null) return false;

            store.IsActive = false;
            store.ModifiedAt = DateTime.UtcNow;

            await _storeRepository.UpdateAsync(store);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting store ID: {StoreId}", id);
            return false;
        }
    }

    public async Task<StoreDetailDto?> GetByCurrentSellerAsync(long userId)
    {
        try
        {
            var sellers = await _sellerRepository.GetAllAsync();
            var seller = sellers.FirstOrDefault(s => s.UserId == userId);
            if (seller == null)
            {
                return null;
            }

            var stores = await _storeRepository.GetAllAsync();
            var store = stores.FirstOrDefault(s => s.SellerId == seller.Id);
            if (store == null)
            {
                return null;
            }

            return await GetByIdAsync(store.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current seller's store");
            return null;
        }
    }

    public async Task<StoreStatsDto> GetStatsAsync()
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            var sellers = await _sellerRepository.GetAllAsync();

            return new StoreStatsDto
            {
                TotalStores = stores.Count(),
                ActiveStores = stores.Count(s => s.IsActive),
                InactiveStores = stores.Count(s => !s.IsActive),
                TotalSellers = sellers.Count(),
                ActiveSellers = sellers.Count(s => s.IsActive),
                AverageProductsPerStore = 0, // TODO: Calculate from products
                NewStoresThisMonth = stores.Count(s => s.CreatedAt >= DateTime.UtcNow.AddMonths(-1)),
                NewStoresThisWeek = stores.Count(s => s.CreatedAt >= DateTime.UtcNow.AddDays(-7))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store stats");
            return new StoreStatsDto();
        }
    }

    public async Task<IEnumerable<StoreDto>> GetActiveStoresAsync()
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            return stores.Where(s => s.IsActive).Select(s => new StoreDto
            {
                Id = s.Id,
                SellerId = s.SellerId,
                Name = s.Name,
                Slug = s.Slug,
                LogoUrl = s.LogoUrl,
                BannerUrl = s.BannerUrl,
                Description = s.Description,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                ModifiedAt = s.ModifiedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active stores");
            return Enumerable.Empty<StoreDto>();
        }
    }

    public async Task<IEnumerable<StoreDto>> GetPendingApprovalAsync()
    {
        try
        {
            var stores = await _storeRepository.GetAllAsync();
            return stores.Where(s => !s.IsActive).Select(s => new StoreDto
            {
                Id = s.Id,
                SellerId = s.SellerId,
                Name = s.Name,
                Slug = s.Slug,
                LogoUrl = s.LogoUrl,
                BannerUrl = s.BannerUrl,
                Description = s.Description,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                ModifiedAt = s.ModifiedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending approval stores");
            return Enumerable.Empty<StoreDto>();
        }
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant()
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u");

        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');
        
        return slug;
    }
}
