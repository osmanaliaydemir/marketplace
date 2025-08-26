using Application.Abstractions;
using Application.DTOs.Stores;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class StoreApplicationService : IStoreApplicationService
{
    private readonly IStoreApplicationRepository _repository;

    public StoreApplicationService(IStoreApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<StoreApplication> SubmitAsync(StoreApplication application)
    {
        // Implementation for existing method
        return await _repository.AddAsync(application);
    }

    public async Task<bool> ApproveAsync(long applicationId, long approvedByUserId)
    {
        // Implementation for existing method
        var application = await _repository.GetByIdAsync(applicationId);
        if (application == null) return false;
        
        application.Status = StoreApplicationStatus.Approved;
        application.ApprovedAt = DateTime.UtcNow;
        application.ApprovedByUserId = approvedByUserId;
        
        await _repository.UpdateAsync(application);
        return true;
    }

    public async Task<bool> RejectAsync(long applicationId, string reason, long rejectedByUserId)
    {
        // Implementation for existing method
        var application = await _repository.GetByIdAsync(applicationId);
        if (application == null) return false;
        
        application.Status = StoreApplicationStatus.Rejected;
        application.RejectedAt = DateTime.UtcNow;
        application.RejectedByUserId = rejectedByUserId;
        application.RejectionReason = reason;
        
        await _repository.UpdateAsync(application);
        return true;
    }

    public async Task<StoreApplication?> GetAsync(long id)
    {
        // Implementation for existing method
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<StoreApplication>> ListPendingAsync()
    {
        // Implementation for existing method
        return await _repository.GetByStatusAsync("Pending");
    }

    public async Task<StoreApplicationSearchResponse> SearchAsync(StoreApplicationSearchRequest request)
    {
        // Implementation for existing method
        var applications = await _repository.SearchAsync(request);
        return new StoreApplicationSearchResponse
        {
            Applications = applications.Select(a => new StoreApplicationListDto
            {
                Id = a.Id,
                BusinessName = a.BusinessName,
                BusinessType = a.BusinessType,
                Status = a.Status.ToString(),
                SubmittedAt = a.SubmittedAt,
                ContactName = a.ContactName,
                PhoneNumber = a.PhoneNumber,
                City = a.City
            }).ToList(),
            TotalCount = applications.Count()
        };
    }

    public async Task<StoreApplicationStatsDto> GetStatsAsync()
    {
        // Implementation for existing method
        var pending = await _repository.GetByStatusAsync("Pending");
        var approved = await _repository.GetByStatusAsync("Approved");
        var rejected = await _repository.GetByStatusAsync("Rejected");
        
        return new StoreApplicationStatsDto
        {
            PendingCount = pending.Count(),
            ApprovedCount = approved.Count(),
            RejectedCount = rejected.Count(),
            TotalCount = pending.Count() + approved.Count() + rejected.Count()
        };
    }

    public async Task<IEnumerable<StoreApplication>> GetByStatusAsync(string status)
    {
        // Implementation for existing method
        return await _repository.GetByStatusAsync(status);
    }

    public async Task<IEnumerable<StoreApplication>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        // Implementation for existing method
        return await _repository.GetByDateRangeAsync(from, to);
    }

    // New methods for StoreApplicationsController
    public async Task<StoreApplicationDetailDto> CreateApplicationAsync(StoreApplicationCreateRequest request)
    {
        var application = new StoreApplication
        {
            BusinessName = request.BusinessName,
            BusinessType = request.BusinessType,
            TaxNumber = request.TaxNumber,
            BusinessLicense = request.BusinessLicense,
            BusinessDescription = request.BusinessDescription,
            Website = request.Website,
            SocialMedia = request.SocialMedia,
            PrimaryCategory = request.PrimaryCategory,
            SecondaryCategory = request.SecondaryCategory,
            ProductCount = request.ProductCount,
            ExpectedRevenue = request.ExpectedRevenue,
            Experience = request.Experience,
            ProductDescription = request.ProductDescription,
            ContactName = request.ContactName,
            PhoneNumber = request.PhoneNumber,
            City = request.City,
            Address = request.Address,
            TermsAccepted = request.TermsAccepted,
            Newsletter = request.Newsletter,
            Status = StoreApplicationStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(application);
        
        return new StoreApplicationDetailDto
        {
            Id = created.Id,
            BusinessName = created.BusinessName,
            BusinessType = created.BusinessType,
            TaxNumber = created.TaxNumber,
            BusinessLicense = created.BusinessLicense,
            BusinessDescription = created.BusinessDescription,
            Website = created.Website,
            SocialMedia = created.SocialMedia,
            PrimaryCategory = created.PrimaryCategory,
            SecondaryCategory = created.SecondaryCategory,
            ProductCount = created.ProductCount,
            ExpectedRevenue = created.ExpectedRevenue,
            Experience = created.Experience,
            ProductDescription = created.ProductDescription,
            ContactName = created.ContactName,
            PhoneNumber = created.PhoneNumber,
            City = created.City,
            Address = created.Address,
            TermsAccepted = created.TermsAccepted,
            Newsletter = created.Newsletter,
            Status = created.Status.ToString(),
            SubmittedAt = created.SubmittedAt,
            ApprovedAt = created.ApprovedAt,
            RejectedAt = created.RejectedAt,
            RejectionReason = created.RejectionReason,
            IsSuccess = true,
            Data = null,
            ErrorMessage = null
        };
    }

    public async Task<IEnumerable<StoreApplicationListDto>> GetApplicationsAsync(int page = 1, int pageSize = 20)
    {
        var applications = await _repository.GetAllAsync();
        
        var pagedApplications = applications
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        return pagedApplications.Select(a => new StoreApplicationListDto
        {
            Id = a.Id,
            BusinessName = a.BusinessName,
            BusinessType = a.BusinessType,
            Status = a.Status.ToString(),
            SubmittedAt = a.SubmittedAt,
            ContactName = a.ContactName,
            PhoneNumber = a.PhoneNumber,
            City = a.City
        });
    }

    public async Task<StoreApplicationDetailDto> GetApplicationByIdAsync(long id)
    {
        var application = await _repository.GetByIdAsync(id);
        if (application == null) 
        {
            return new StoreApplicationDetailDto
            {
                IsSuccess = false,
                ErrorMessage = "Mağaza başvurusu bulunamadı"
            };
        }
        
        var dto = new StoreApplicationDetailDto
        {
            Id = application.Id,
            BusinessName = application.BusinessName,
            BusinessType = application.BusinessType,
            TaxNumber = application.TaxNumber,
            BusinessLicense = application.BusinessLicense,
            BusinessDescription = application.BusinessDescription,
            Website = application.Website,
            SocialMedia = application.SocialMedia,
            PrimaryCategory = application.PrimaryCategory,
            SecondaryCategory = application.SecondaryCategory,
            ProductCount = application.ProductCount,
            ExpectedRevenue = application.ExpectedRevenue,
            Experience = application.Experience,
            ProductDescription = application.ProductDescription,
            ContactName = application.ContactName,
            PhoneNumber = application.PhoneNumber,
            City = application.City,
            Address = application.Address,
            TermsAccepted = application.TermsAccepted,
            Newsletter = application.Newsletter,
            Status = application.Status.ToString(),
            SubmittedAt = application.SubmittedAt,
            ApprovedAt = application.ApprovedAt,
            RejectedAt = application.RejectedAt,
            RejectionReason = application.RejectionReason,
            IsSuccess = true,
            Data = null,
            ErrorMessage = null
        };
        
        return dto;
    }

    public async Task<StoreApplicationDetailDto> UpdateApplicationAsync(long id, StoreApplicationUpdateRequest request)
    {
        var application = await _repository.GetByIdAsync(id);
        if (application == null) 
        {
            return new StoreApplicationDetailDto
            {
                IsSuccess = false,
                ErrorMessage = "Mağaza başvurusu bulunamadı"
            };
        }
        
        // Update fields
        application.BusinessName = request.BusinessName;
        application.BusinessType = request.BusinessType;
        application.TaxNumber = request.TaxNumber;
        application.BusinessLicense = request.BusinessLicense;
        application.BusinessDescription = request.BusinessDescription;
        application.Website = request.Website;
        application.SocialMedia = request.SocialMedia;
        application.PrimaryCategory = request.PrimaryCategory;
        application.SecondaryCategory = request.SecondaryCategory;
        application.ProductCount = request.ProductCount;
        application.ExpectedRevenue = request.ExpectedRevenue;
        application.Experience = request.Experience;
        application.ProductDescription = request.ProductDescription;
        application.ContactName = request.ContactName;
        application.PhoneNumber = request.PhoneNumber;
        application.City = request.City;
        application.Address = request.Address;
        
        await _repository.UpdateAsync(application);
        
        var updatedDto = await GetApplicationByIdAsync(id);
        updatedDto.IsSuccess = true;
        updatedDto.Data = null;
        updatedDto.ErrorMessage = null;
        
        return updatedDto;
    }

    public async Task<StoreApplicationDetailDto> ApproveApplicationAsync(long id, StoreApplicationApprovalRequest request)
    {
        var application = await _repository.GetByIdAsync(id);
        if (application == null) 
        {
            return new StoreApplicationDetailDto
            {
                IsSuccess = false,
                ErrorMessage = "Mağaza başvurusu bulunamadı"
            };
        }
        
        application.Status = StoreApplicationStatus.Approved;
        application.ApprovedAt = DateTime.UtcNow;
        application.ApprovedByUserId = request.ApprovedByUserId;
        application.ApprovalNotes = request.ApprovalNotes;
        
        await _repository.UpdateAsync(application);
        
        var approvedDto = await GetApplicationByIdAsync(id);
        approvedDto.IsSuccess = true;
        approvedDto.Data = null;
        approvedDto.ErrorMessage = null;
        
        return approvedDto;
    }

    public async Task<StoreApplicationDetailDto> RejectApplicationAsync(long id, StoreApplicationRejectionRequest request)
    {
        var application = await _repository.GetByIdAsync(id);
        if (application == null) 
        {
            return new StoreApplicationDetailDto
            {
                IsSuccess = false,
                ErrorMessage = "Mağaza başvurusu bulunamadı"
            };
        }
        
        application.Status = StoreApplicationStatus.Rejected;
        application.RejectedAt = DateTime.UtcNow;
        application.RejectedByUserId = request.RejectedByUserId;
        application.RejectionReason = request.RejectionReason;
        
        await _repository.UpdateAsync(application);
        
        var rejectedDto = await GetApplicationByIdAsync(id);
        rejectedDto.IsSuccess = true;
        rejectedDto.Data = null;
        rejectedDto.ErrorMessage = null;
        
        return rejectedDto;
    }

    public async Task<StoreApplicationDetailDto> DeleteApplicationAsync(long id)
    {
        var application = await _repository.GetByIdAsync(id);
        if (application == null) 
        {
            return new StoreApplicationDetailDto
            {
                IsSuccess = false,
                ErrorMessage = "Mağaza başvurusu bulunamadı"
            };
        }
        
        await _repository.DeleteAsync(id);
        
        return new StoreApplicationDetailDto
        {
            IsSuccess = true,
            Data = null,
            ErrorMessage = null
        };
    }
}
