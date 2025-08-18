using Application.DTOs.Stores;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Api.Controllers;

[ApiController]
[Route("api/store-applications")]
public sealed class StoreApplicationsController : ControllerBase
{
	private readonly IStoreApplicationService _service;
	private readonly IValidator<StoreApplicationCreateRequest> _validator;

	public StoreApplicationsController(
		IStoreApplicationService service,
		IValidator<StoreApplicationCreateRequest> validator)
	{
		_service = service;
		_validator = validator;
	}

	[HttpPost]
	[Authorize(Roles = "Customer,Admin")]
	public async Task<ActionResult<StoreApplicationDetailDto>> Submit([FromBody] StoreApplicationCreateRequest req)
	{
		// Validation
		var validationResult = await _validator.ValidateAsync(req);
		if (!validationResult.IsValid)
		{
			var errors = validationResult.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
			return BadRequest(new { Errors = errors });
		}

		try
		{
			var entity = new StoreApplication
			{
				StoreName = req.StoreName,
				Slug = req.Slug,
				ContactEmail = req.ContactEmail,
				ContactPhone = req.ContactPhone,
				Description = req.Description,
				BusinessAddress = req.BusinessAddress,
				TaxNumber = req.TaxNumber
			};

			var created = await _service.SubmitAsync(entity);
			var dto = new StoreApplicationDetailDto
			{
				Id = created.Id,
				StoreName = created.StoreName,
				Slug = created.Slug,
				Description = created.Description,
				ContactEmail = created.ContactEmail,
				ContactPhone = created.ContactPhone,
				BusinessAddress = created.BusinessAddress,
				TaxNumber = created.TaxNumber,
				Status = created.Status.ToString(),
				CreatedAt = created.CreatedAt
			};
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Mağaza başvurusu oluşturulurken bir hata oluştu", Error = ex.Message });
		}
	}

	[HttpGet("pending")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<IEnumerable<StoreApplicationListDto>>> ListPending()
	{
		try
		{
			var list = await _service.ListPendingAsync();
			var dtos = list.Select(x => new StoreApplicationListDto
			{
				Id = x.Id,
				StoreName = x.StoreName,
				Slug = x.Slug,
				ContactEmail = x.ContactEmail,
				Status = x.Status.ToString(),
				CreatedAt = x.CreatedAt
			});
			return Ok(dtos);
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Bekleyen başvurular listelenirken bir hata oluştu", Error = ex.Message });
		}
	}

	[HttpPost("{id}/approve")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Approve(long id)
	{
		try
		{
			// TODO: Auth context'ten admin user ID alınmalı
			var adminUserId = GetCurrentUserId();
			var ok = await _service.ApproveAsync(id, adminUserId);
			if (!ok) 
                return BadRequest(new { Message = "Başvuru onaylanamadı. Durumunu kontrol ediniz." });

			return Ok(new { Message = "Başvuru onaylandı"});
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Başvuru onaylanırken bir hata oluştu", Error = ex.Message });
		}
	}

	[HttpPost("{id}/reject")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Reject(long id, [FromBody] string reason)
	{
		if (string.IsNullOrWhiteSpace(reason))
			return BadRequest(new { Message = "Red sebebi belirtilmelidir" });

		try
		{
			var adminUserId = GetCurrentUserId();
			var ok = await _service.RejectAsync(id, reason, adminUserId);
			if (!ok) return BadRequest(new { Message = "Başvuru reddedilemedi. Durumunu kontrol ediniz." });
			return NoContent();
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Başvuru reddedilirken bir hata oluştu", Error = ex.Message });
		}
	}

	[HttpGet("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<StoreApplicationDetailDto>> GetById(long id)
	{
		try
		{
			var entity = await _service.GetAsync(id);
			if (entity is null) return NotFound(new { Message = "Başvuru bulunamadı" });
			
			return Ok(new StoreApplicationDetailDto
			{
				Id = entity.Id,
				StoreName = entity.StoreName,
				Slug = entity.Slug,
				Description = entity.Description,
				ContactEmail = entity.ContactEmail,
				ContactPhone = entity.ContactPhone,
				BusinessAddress = entity.BusinessAddress,
				TaxNumber = entity.TaxNumber,
				Status = entity.Status.ToString(),
				CreatedAt = entity.CreatedAt,
				ApprovedAt = entity.ApprovedAt,
				RejectionReason = entity.RejectionReason
			});
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Başvuru bilgileri alınırken bir hata oluştu", Error = ex.Message });
		}
	}

	// Dashboard endpoints
	[HttpGet("search")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<StoreApplicationSearchResponse>> Search([FromQuery] StoreApplicationSearchRequest request)
	{
		try
		{
			// Validation
			if (request.Page < 1) request.Page = 1;
			if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 20;

			var result = await _service.SearchAsync(request);
			return Ok(result);
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Arama yapılırken bir hata oluştu", Error = ex.Message });
		}
	}

	[HttpGet("stats")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<StoreApplicationStatsDto>> GetStats()
	{
		try
		{
			var stats = await _service.GetStatsAsync();
			return Ok(stats);
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "İstatistikler alınırken bir hata oluştu", Error = ex.Message });
		}
	}

	[HttpGet("by-status/{status}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<IEnumerable<StoreApplicationListDto>>> GetByStatus(string status)
	{
		try
		{
			var list = await _service.GetByStatusAsync(status);
			var dtos = list.Select(x => new StoreApplicationListDto
			{
				Id = x.Id,
				StoreName = x.StoreName,
				Slug = x.Slug,
				ContactEmail = x.ContactEmail,
				Status = x.Status.ToString(),
				CreatedAt = x.CreatedAt
			});
			return Ok(dtos);
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Durum bazlı listeleme yapılırken bir hata oluştu", Error = ex.Message });
		}
	}

	[HttpGet("by-date-range")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<IEnumerable<StoreApplicationListDto>>> GetByDateRange(
		[FromQuery] DateTime from, 
		[FromQuery] DateTime to)
	{
		try
		{
			if (from > to)
				return BadRequest(new { Message = "Başlangıç tarihi bitiş tarihinden büyük olamaz" });

			var list = await _service.GetByDateRangeAsync(from, to);
			var dtos = list.Select(x => new StoreApplicationListDto
			{
				Id = x.Id,
				StoreName = x.StoreName,
				Slug = x.Slug,
				ContactEmail = x.ContactEmail,
				Status = x.Status.ToString(),
				CreatedAt = x.CreatedAt
			});
			return Ok(dtos);
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { Message = "Tarih aralığı listeleme yapılırken bir hata oluştu", Error = ex.Message });
		}
	}

	// TODO: Gerçek authentication context'ten user ID alınacak
	private long GetCurrentUserId()
	{
		// Şimdilik hardcoded, gerçek implementasyonda JWT'den alınacak
		return 1;
	}
}
