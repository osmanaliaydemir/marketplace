using Api.DTOs.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Abstractions;

namespace Api.Controllers;

[ApiController]
[Route("api/stores")]
[Authorize(Roles = "Admin")]
public sealed class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;
    private readonly ILogger<StoresController> _logger;

    public StoresController(IStoreService storeService, ILogger<StoresController> logger)
    {
        _storeService = storeService;
        _logger = logger;
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<StoreDetailDto>> GetMine()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var store = await _storeService.GetByCurrentSellerAsync(userId);
            if (store == null)
            {
                return NotFound(new { Message = "Mağaza bulunamadı" });
            }

            return Ok(store);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current seller's store");
            return StatusCode(500, new { Message = "Mağaza bilgileri alınırken bir hata oluştu" });
        }
    }

    [HttpPut("mine")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<StoreDetailDto>> UpdateMine([FromBody] UpdateMyStoreRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Kullanıcı kimliği doğrulanamadı" });
            }

            var store = await _storeService.GetByCurrentSellerAsync(userId);
            if (store == null)
            {
                return NotFound(new { Message = "Mağaza bulunamadı" });
            }

            var updateRequest = new Application.DTOs.Stores.StoreUpdateRequest
            {
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                Phone = request.Phone,
                Email = request.Email,
                Website = request.Website,
                Address = request.Address,
                Currency = request.Currency,
                Language = request.Language,
                IsActive = request.IsActive
            };

            var updatedStore = await _storeService.UpdateAsync(store.Id, updateRequest);
            if (updatedStore == null)
            {
                return BadRequest(new { Message = "Mağaza güncellenemedi" });
            }

            return Ok(updatedStore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating current seller's store");
            return StatusCode(500, new { Message = "Mağaza güncellenirken bir hata oluştu" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StoreListDto>>> GetAll()
    {
        try
        {
            var request = new Application.DTOs.Stores.StoreListRequest
            {
                Page = 1,
                PageSize = 1000
            };

            var result = await _storeService.ListAsync(request);
            var dtos = result.Items.Select(store => new StoreListDto
            {
                Id = store.Id,
                Name = store.Name,
                Slug = store.Slug,
                LogoUrl = store.LogoUrl,
                IsActive = store.IsActive,
                SellerName = store.Seller?.FullName ?? "Bilinmeyen Satıcı",
                ProductCount = 0, // TODO: Product count hesaplanacak
                CreatedAt = store.CreatedAt
            });

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all stores");
            return StatusCode(500, new { Message = "Mağazalar listelenirken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StoreDetailDto>> GetById(long id)
    {
        try
        {
            var store = await _storeService.GetByIdAsync(id);
            if (store == null)
                return NotFound(new { Message = "Mağaza bulunamadı" });

            return Ok(store);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store by ID: {StoreId}", id);
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

            var searchRequest = new Application.DTOs.Stores.StoreSearchRequest
            {
                SearchTerm = request.SearchTerm,
                IsActive = request.IsActive,
                SellerId = request.SellerId,
                Page = request.Page,
                PageSize = request.PageSize
            };

            var result = await _storeService.SearchAsync(searchRequest);

            var response = new StoreSearchResponse
            {
                Items = result.Items.Select(store => new StoreListDto
                {
                    Id = store.Id,
                    Name = store.Name,
                    Slug = store.Slug,
                    LogoUrl = store.LogoUrl,
                    IsActive = store.IsActive,
                    SellerName = store.Seller?.FullName ?? "Bilinmeyen Satıcı",
                    ProductCount = 0, // TODO: Product count hesaplanacak
                    CreatedAt = store.CreatedAt
                }),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.PageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching stores");
            return StatusCode(500, new { Message = "Mağaza araması yapılırken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<StoreStatsDto>> GetStats()
    {
        try
        {
            var stats = await _storeService.GetStatsAsync();

            var response = new StoreStatsDto
            {
                TotalStores = stats.TotalStores,
                ActiveStores = stats.ActiveStores,
                InactiveStores = stats.InactiveStores,
                TotalSellers = stats.TotalSellers,
                ActiveSellers = stats.ActiveSellers,
                AverageProductsPerStore = stats.AverageProductsPerStore,
                NewStoresThisMonth = stats.NewStoresThisMonth,
                NewStoresThisWeek = stats.NewStoresThisWeek
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store stats");
            return StatusCode(500, new { Message = "Mağaza istatistikleri alınırken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStoreStatusRequest request)
    {
        try
        {
            var success = await _storeService.SetActiveAsync(id, request.IsActive);
            if (!success)
            {
                return BadRequest(new { Message = "Mağaza durumu güncellenemedi" });
            }

            return Ok(new { Message = "Mağaza durumu güncellendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating store status ID: {StoreId}", id);
            return StatusCode(500, new { Message = "Mağaza durumu güncellenirken bir hata oluştu", Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var success = await _storeService.DeleteAsync(id);
            if (!success)
            {
                return BadRequest(new { Message = "Mağaza silinemedi" });
            }

            return Ok(new { Message = "Mağaza silindi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting store ID: {StoreId}", id);
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

public sealed class UpdateMyStoreRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Language { get; set; } = "tr";
    public bool IsActive { get; set; } = true;
}
