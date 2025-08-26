using Application.Abstractions;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public sealed class AppUserService : IAppUserService
{
    private readonly IAppUserRepository _userRepository;
    private readonly ILogger<AppUserService> _logger;

    public AppUserService(IAppUserRepository userRepository, ILogger<AppUserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> GetByIdAsync(long id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            throw;
        }
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        try
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var user = await _userRepository.GetByUsernameAsync(username);
            return user != null ? MapToDto(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username {Username}", username);
            throw;
        }
    }

    public async Task<UserDto?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        try
        {
            if (string.IsNullOrEmpty(emailOrUsername))
                return null;

            var user = await _userRepository.GetByEmailOrUsernameAsync(emailOrUsername);
            return user != null ? MapToDto(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email or username {EmailOrUsername}", emailOrUsername);
            throw;
        }
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Email ve username unique kontrolü
            if (!await IsEmailUniqueAsync(request.Email))
                throw new InvalidOperationException($"Email {request.Email} already exists");

            if (!await IsUsernameUniqueAsync(request.FullName))
                throw new InvalidOperationException($"Username {request.FullName} already exists");

            // Entity oluştur
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = HashPassword(request.Password),
                Role = Enum.Parse<UserRole>(request.Role),
                IsActive = true
            };

            var createdUser = await _userRepository.AddAsync(user);
            return MapToDto(createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", request?.Email);
            throw;
        }
    }

    public async Task<UserDto> UpdateAsync(UpdateUserRequest request)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
                throw new InvalidOperationException($"User with ID {request.Id} not found");

            // Email unique kontrolü (kendisi hariç)
            if (!await IsEmailUniqueAsync(request.Email, request.Id))
                throw new InvalidOperationException($"Email {request.Email} already exists");

            // Username unique kontrolü (kendisi hariç)
            if (!await IsUsernameUniqueAsync(request.FullName, request.Id))
                throw new InvalidOperationException($"Username {request.FullName} already exists");

            // Entity güncelle
            existingUser.Email = request.Email;
            existingUser.FullName = request.FullName;
            existingUser.Role = Enum.Parse<UserRole>(request.Role);
            existingUser.IsActive = request.IsActive;

            // Şifre değişikliği varsa güncelle
            if (!string.IsNullOrEmpty(request.Password))
            {
                existingUser.PasswordHash = HashPassword(request.Password);
            }

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return MapToDto(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", request?.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<bool> IsEmailUniqueAsync(string email, long? excludeUserId = null)
    {
        try
        {
            return await _userRepository.IsEmailUniqueAsync(email, excludeUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email uniqueness for {Email}", email);
            throw;
        }
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, long? excludeUserId = null)
    {
        try
        {
            return await _userRepository.IsUsernameUniqueAsync(username, excludeUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username uniqueness for {Username}", username);
            throw;
        }
    }

    public async Task<bool> VerifyPasswordAsync(string emailOrUsername, string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(emailOrUsername);
            if (user == null)
                return false;

            var hashedPassword = HashPassword(password);
            return user.PasswordHash.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password for {EmailOrUsername}", emailOrUsername);
            throw;
        }
    }

    public async Task<bool> IsUserActiveAsync(string emailOrUsername)
    {
        try
        {
            var user = await _userRepository.GetByEmailOrUsernameAsync(emailOrUsername);
            return user?.IsActive ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user active status for {EmailOrUsername}", emailOrUsername);
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return false;

            // Mevcut şifreyi doğrula
            if (!await VerifyPasswordAsync(user.Email, request.CurrentPassword))
                return false;

            // Yeni şifreyi hash'le ve güncelle
            user.PasswordHash = HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", request?.UserId);
            throw;
        }
    }

    public async Task<UserSearchResponse> SearchAsync(UserSearchRequest request)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // TODO: Repository'de search metodu implement edilecek
            // Şimdilik basit bir implementasyon
            var allUsers = await _userRepository.GetAllAsync();
            
            var filteredUsers = allUsers.AsQueryable();
            
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                filteredUsers = filteredUsers.Where(u => 
                    u.Email.Contains(request.SearchTerm) || 
                    u.FullName.Contains(request.SearchTerm));
            }
            
            if (!string.IsNullOrEmpty(request.Role))
            {
                filteredUsers = filteredUsers.Where(u => u.Role.ToString() == request.Role);
            }
            
            if (request.IsActive.HasValue)
            {
                filteredUsers = filteredUsers.Where(u => u.IsActive == request.IsActive.Value);
            }

            var totalCount = filteredUsers.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            
            var pagedUsers = filteredUsers
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new UserSearchResponse
            {
                Users = pagedUsers.Select(MapToDto),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            throw;
        }
    }

    private static UserDto MapToDto(AppUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt
        };
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
