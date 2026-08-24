// ITAM.AppCore/Interfaces/IUserService.cs
using ITAM.AppCore.DTOs.Identity;
using ITAM.Domain.Entities.Identity;


namespace ITAM.AppCore.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(long id);
        Task<User> CreateAsync(CreateUserDto dto);
        Task UpdateAsync(UpdateUserDto dto);
        Task SetActiveAsync(long userId, bool isActive);
        Task ChangePasswordAsync(ChangePasswordDto dto);
        Task ResetPasswordAsync(long userId, string newPassword);
        Task<bool> IsUsernameExistsAsync(string username, long? excludeUserId = null);
    }
}