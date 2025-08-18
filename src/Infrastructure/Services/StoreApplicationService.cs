using Application.Abstractions;
using Application.DTOs.Stores;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public sealed class StoreApplicationService
{
    private readonly IStoreUnitOfWork _unitOfWork;
    private readonly ILogger<StoreApplicationService> _logger;

    public StoreApplicationService(IStoreUnitOfWork unitOfWork, ILogger<StoreApplicationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StoreApplication> SubmitAsync(StoreApplication application)
    {
        try
        {
            _logger.LogInformation("Store application submitted for {StoreName}", application.StoreName);

            // Transaction başlat
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Başvuruyu kaydet
                application = await _unitOfWork.StoreApplications.AddAsync(application);

                // Başvuru sahibi kullanıcıyı bul veya oluştur
                var applicantUser = await FindOrCreateUserAsync(application.ContactEmail, application.StoreName);

                // Başvuru sahibini seller olarak kaydet
                var seller = new Seller
                {
                    UserId = applicantUser.Id,
                    CommissionRate = 0.10m, // Varsayılan %10 komisyon
                    IsActive = false, // Onaylanana kadar pasif
                    CreatedAt = DateTime.UtcNow
                };

                seller = await _unitOfWork.Sellers.AddAsync(seller);

                // Başvuruya seller ID'sini ekle
                application.SellerId = seller.Id;
                await _unitOfWork.StoreApplications.UpdateAsync(application);

                // Transaction'ı commit et
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Store application {Id} submitted successfully", application.Id);
                return application;
            }
            catch
            {
                // Hata durumunda rollback
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting store application for {StoreName}", application.StoreName);
            throw;
        }
    }

    public async Task<StoreApplication> ApproveAsync(long applicationId, long approvedByUserId)
    {
        try
        {
            _logger.LogInformation("Approving store application {Id}", applicationId);

            // Transaction başlat
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Başvuruyu bul
                var application = await _unitOfWork.StoreApplications.GetByIdAsync(applicationId);
                if (application == null)
                    throw new InvalidOperationException($"Store application {applicationId} not found");

                if (application.Status != StoreApplicationStatus.Pending)
                    throw new InvalidOperationException($"Store application {applicationId} is not pending");

                // Başvuruyu onayla
                application.Status = StoreApplicationStatus.Approved;
                application.ApprovedAt = DateTime.UtcNow;
                application.ApprovedByUserId = approvedByUserId;
                application.ModifiedAt = DateTime.UtcNow;

                await _unitOfWork.StoreApplications.UpdateAsync(application);

                // Mağaza oluştur
                var store = new Store
                {
                    SellerId = application.SellerId!.Value,
                    Name = application.StoreName,
                    Slug = application.Slug,
                    LogoUrl = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                store = await _unitOfWork.Stores.AddAsync(store);

                // Seller'ı aktif yap
                var seller = await _unitOfWork.Sellers.GetByIdAsync(application.SellerId.Value);
                if (seller != null)
                {
                    seller.IsActive = true;
                    seller.ModifiedAt = DateTime.UtcNow;
                    await _unitOfWork.Sellers.UpdateAsync(seller);
                }

                // Transaction'ı commit et
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Store application {Id} approved successfully, store {StoreId} created", applicationId, store.Id);
                return application;
            }
            catch
            {
                // Hata durumunda rollback
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving store application {Id}", applicationId);
            throw;
        }
    }

    public async Task<StoreApplication> RejectAsync(long applicationId, string reason, long rejectedByUserId)
    {
        try
        {
            _logger.LogInformation("Rejecting store application {Id}", applicationId);

            var application = await _unitOfWork.StoreApplications.GetByIdAsync(applicationId);
            if (application == null)
                throw new InvalidOperationException($"Store application {applicationId} not found");

            if (application.Status != StoreApplicationStatus.Pending)
                throw new InvalidOperationException($"Store application {applicationId} is not pending");

            // Başvuruyu reddet
            application.Status = StoreApplicationStatus.Rejected;
            application.RejectionReason = reason;
            application.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.StoreApplications.UpdateAsync(application);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Store application {Id} rejected successfully", applicationId);
            return application;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting store application {Id}", applicationId);
            throw;
        }
    }

    private async Task<AppUser> FindOrCreateUserAsync(string email, string fullName)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var existingUser = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
            return existingUser;

        // Yeni kullanıcı oluştur
        var newUser = new AppUser
        {
            Email = email,
            FullName = fullName,
            PasswordHash = string.Empty, // Şifre daha sonra set edilecek
            Role = "Seller",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return await _unitOfWork.Users.AddAsync(newUser);
    }
}
