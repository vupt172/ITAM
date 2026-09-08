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
        Task<List<long>> GetAccessiblePhongBanIdsAsync(long userId);
        Task SetPhongBanAccessAsync(long userId, long? defaultPhongBanId, List<long> accessiblePhongBanIds);
        Task ChangePasswordAsync(ChangePasswordDto dto);
        Task SetDefaultPhongBanAsync(long userId, long phongBanId);
        Task ResetPasswordAsync(long userId, string newPassword);
        Task<bool> IsUsernameExistsAsync(string username, long? excludeUserId = null);
        Task DeleteAsync(long userId);

    }
}