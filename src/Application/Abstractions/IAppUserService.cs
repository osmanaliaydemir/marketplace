using Application.DTOs.Users;

namespace Application.Abstractions;

public interface IAppUserService
{
    Task<UserDto?> GetByIdAsync(long id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByUsernameAsync(string emailOrUsername);
    Task<UserDto?> GetByEmailOrUsernameAsync(string emailOrUsername);
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto> UpdateAsync(UpdateUserRequest request);
    Task<bool> DeleteAsync(long id);
    Task<bool> IsEmailUniqueAsync(string email, long? excludeUserId = null);
    Task<bool> IsUsernameUniqueAsync(string username, long? excludeUserId = null);
    Task<bool> VerifyPasswordAsync(string emailOrUsername, string password);
    Task<bool> IsUserActiveAsync(string emailOrUsername);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
    Task<UserSearchResponse> SearchAsync(UserSearchRequest request);
}
