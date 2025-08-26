using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Stores;
using Application.Abstractions;
using Application.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreApplicationsController : ControllerBase
    {
        private readonly IStoreApplicationService _storeApplicationService;
        private readonly ILogger<StoreApplicationsController> _logger;

        public StoreApplicationsController(
            IStoreApplicationService storeApplicationService,
            ILogger<StoreApplicationsController> logger)
        {
            _storeApplicationService = storeApplicationService;
            _logger = logger;
        }

        /// <summary>
        /// Yeni mağaza başvurusu oluşturur
        /// </summary>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateApplication([FromBody] StoreApplicationCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Geçersiz veri", 
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                    });
                }

                var result = await _storeApplicationService.CreateApplicationAsync(request);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Mağaza başvurusu oluşturuldu: {BusinessName}", request.BusinessName);
                    
                    return Ok(new { 
                        success = true, 
                        message = "Mağaza başvurusu başarıyla oluşturuldu",
                        data = result.Data
                    });
                }
                else
                {
                    _logger.LogWarning("Mağaza başvurusu oluşturulamadı: {BusinessName}, Hata: {Error}", 
                        request.BusinessName, result.ErrorMessage);
                    
                    return BadRequest(new { 
                        success = false, 
                        message = result.ErrorMessage ?? "Mağaza başvurusu oluşturulamadı" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mağaza başvurusu oluşturulurken hata oluştu: {BusinessName}", request.BusinessName);
                
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin." 
                });
            }
        }

        /// <summary>
        /// Mağaza başvurularını listeler (Admin için)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _storeApplicationService.GetApplicationsAsync(page, pageSize);
                
                return Ok(new { 
                    success = true, 
                    data = result,
                    pagination = new {
                        page,
                        pageSize,
                        totalCount = result.Count(),
                        totalPages = (int)Math.Ceiling((double)result.Count() / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mağaza başvuruları alınırken hata oluştu");
                
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası oluştu" 
                });
            }
        }

        /// <summary>
        /// Belirli bir mağaza başvurusunu getirir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(long id)
        {
            try
            {
                var result = await _storeApplicationService.GetApplicationByIdAsync(id);
                
                if (result.IsSuccess && result.Data != null)
                {
                    return Ok(new { 
                        success = true, 
                        data = result.Data 
                    });
                }
                else
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Mağaza başvurusu bulunamadı" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mağaza başvurusu alınırken hata oluştu: {Id}", id);
                
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası oluştu" 
                });
            }
        }

        /// <summary>
        /// Mağaza başvurusunu günceller (Admin için)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication(long id, [FromBody] StoreApplicationUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Geçersiz veri", 
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) 
                    });
                }

                request.Id = id;
                var result = await _storeApplicationService.UpdateApplicationAsync(id, request);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Mağaza başvurusu güncellendi: {Id}", id);
                    
                    return Ok(new { 
                        success = true, 
                        message = "Mağaza başvurusu başarıyla güncellendi",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.ErrorMessage ?? "Mağaza başvurusu güncellenemedi" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mağaza başvurusu güncellenirken hata oluştu: {Id}", id);
                
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası oluştu" 
                });
            }
        }

        /// <summary>
        /// Mağaza başvurusunu onaylar (Admin için)
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveApplication(long id, [FromBody] StoreApplicationApprovalRequest request)
        {
            try
            {
                request.ApplicationId = id;
                var result = await _storeApplicationService.ApproveApplicationAsync(id, request);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Mağaza başvurusu onaylandı: {Id}", id);
                    
                    return Ok(new { 
                        success = true, 
                        message = "Mağaza başvurusu başarıyla onaylandı",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.ErrorMessage ?? "Mağaza başvurusu onaylanamadı" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mağaza başvurusu onaylanırken hata oluştu: {Id}", id);
                
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası oluştu" 
                });
            }
        }

        /// <summary>
        /// Mağaza başvurusunu reddeder (Admin için)
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectApplication(long id, [FromBody] StoreApplicationRejectionRequest request)
        {
            try
            {
                request.ApplicationId = id;
                var result = await _storeApplicationService.RejectApplicationAsync(id, request);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Mağaza başvurusu reddedildi: {Id}", id);
                    
                    return Ok(new { 
                        success = true, 
                        message = "Mağaza başvurusu reddedildi",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.ErrorMessage ?? "Mağaza başvurusu reddedilemedi" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mağaza başvurusu reddedilirken hata oluştu: {Id}", id);
                
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası oluştu" 
                });
            }
        }

        /// <summary>
        /// Mağaza başvurusunu siler (Admin için)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(long id)
        {
            try
            {
                var result = await _storeApplicationService.DeleteApplicationAsync(id);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Mağaza başvurusu silindi: {Id}", id);
                    
                    return Ok(new { 
                        success = true, 
                        message = "Mağaza başvurusu başarıyla silindi" 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = result.ErrorMessage ?? "Mağaza başvurusu silinemedi" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mağaza başvurusu silinirken hata oluştu: {Id}", id);
                
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası oluştu" 
                });
            }
        }
    }
}
