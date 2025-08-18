using Api.DTOs.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence.Repositories;

namespace Api.Controllers;

[ApiController]
[Route("api/stores")]
[Authorize(Roles = "Admin")]
public sealed class StoresController : ControllerBase
{
    private readonly IStoreUnitOfWork _unitOfWork;

    public StoresController(IStoreUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StoreListDto>>> GetAll()
    {
        try
        {
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            var dtos = stores.Select(store =>
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
                    ProductCount = 0, // TODO: Product count hesaplanacak
                    CreatedAt = store.CreatedAt
                };
            });

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Mağazalar listelenirken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StoreDetailDto>> GetById(long id)
    {
        try
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(id);
            if (store == null)
                return NotFound(new { Message = "Mağaza bulunamadı" });

            var seller = await _unitOfWork.Sellers.GetByIdAsync(store.SellerId);
            var user = seller != null ? await _unitOfWork.Users.GetByIdAsync(seller.UserId) : null;

            var dto = new StoreDetailDto
            {
                Id = store.Id,
                SellerId = store.SellerId,
                Name = store.Name,
                Slug = store.Slug,
                LogoUrl = store.LogoUrl,
                IsActive = store.IsActive,
                CreatedAt = store.CreatedAt,
                ModifiedAt = store.ModifiedAt,
                Seller = new SellerInfoDto
                {
                    Id = seller?.Id ?? 0,
                    FullName = user?.FullName ?? "Bilinmeyen Satıcı",
                    Email = user?.Email ?? "",
                    CommissionRate = seller?.CommissionRate ?? 0,
                    IsActive = seller?.IsActive ?? false
                },
                Categories = new List<Api.DTOs.Categories.StoreCategoryListDto>() // TODO: Store categories eklenecek
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Mağaza bilgileri alınırken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<StoreSearchResponse>> Search([FromQuery] StoreSearchRequest request)
    {
        try
        {
            // Validation
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 20;

            var stores = await _unitOfWork.Stores.GetAllAsync();
            var sellers = await _unitOfWork.Sellers.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            // Filtering
            var filteredStores = stores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filteredStores = filteredStores.Where(s => 
                    s.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    s.Slug.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (request.IsActive.HasValue)
            {
                filteredStores = filteredStores.Where(s => s.IsActive == request.IsActive.Value);
            }

            if (request.SellerId.HasValue)
            {
                filteredStores = filteredStores.Where(s => s.SellerId == request.SellerId.Value);
            }

            // Sorting
            filteredStores = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? filteredStores.OrderByDescending(s => s.Name) : filteredStores.OrderBy(s => s.Name),
                "createdat" => request.SortDescending ? filteredStores.OrderByDescending(s => s.CreatedAt) : filteredStores.OrderBy(s => s.CreatedAt),
                "seller" => request.SortDescending ? filteredStores.OrderByDescending(s => s.SellerId) : filteredStores.OrderBy(s => s.SellerId),
                _ => request.SortDescending ? filteredStores.OrderByDescending(s => s.CreatedAt) : filteredStores.OrderBy(s => s.CreatedAt)
            };

            // Paging
            var totalCount = filteredStores.Count();
            var pagedStores = filteredStores
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Convert to DTOs
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
                    ProductCount = 0, // TODO: Product count hesaplanacak
                    CreatedAt = store.CreatedAt
                };
            });

            var response = new StoreSearchResponse
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Mağaza araması yapılırken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<StoreStatsDto>> GetStats()
    {
        try
        {
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var sellers = await _unitOfWork.Sellers.GetAllAsync();

            var stats = new StoreStatsDto
            {
                TotalStores = stores.Count(),
                ActiveStores = stores.Count(s => s.IsActive),
                InactiveStores = stores.Count(s => !s.IsActive),
                TotalSellers = sellers.Count(),
                ActiveSellers = sellers.Count(s => s.IsActive),
                AverageProductsPerStore = 0, // TODO: Product count hesaplanacak
                NewStoresThisMonth = stores.Count(s => s.CreatedAt >= DateTime.UtcNow.AddMonths(-1)),
                NewStoresThisWeek = stores.Count(s => s.CreatedAt >= DateTime.UtcNow.AddDays(-7))
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Mağaza istatistikleri alınırken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStoreStatusRequest request)
    {
        try
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(id);
            if (store == null)
                return NotFound(new { Message = "Mağaza bulunamadı" });

            store.IsActive = request.IsActive;
            store.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Stores.UpdateAsync(store);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { Message = "Mağaza durumu güncellendi" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Mağaza durumu güncellenirken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(id);
            if (store == null)
                return NotFound(new { Message = "Mağaza bulunamadı" });

            await _unitOfWork.Stores.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { Message = "Mağaza silindi" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Mağaza silinirken bir hata oluştu", Error = ex.Message });
        }
    }
}

public sealed class StoreSearchRequest
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public long? SellerId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public sealed class StoreSearchResponse
{
    public IEnumerable<StoreListDto> Items { get; set; } = Enumerable.Empty<StoreListDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public sealed class StoreStatsDto
{
    public int TotalStores { get; set; }
    public int ActiveStores { get; set; }
    public int InactiveStores { get; set; }
    public int TotalSellers { get; set; }
    public int ActiveSellers { get; set; }
    public int AverageProductsPerStore { get; set; }
    public int NewStoresThisMonth { get; set; }
    public int NewStoresThisWeek { get; set; }
}

public sealed class UpdateStoreStatusRequest
{
    public bool IsActive { get; set; }
}
