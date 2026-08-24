using ITAM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Xác thực username/password, trả về User kèm Roles/Features nếu thành công.
        /// Ném AuthenticationException nếu thất bại (sai mật khẩu, user không tồn tại, bị khóa).
        /// </summary>
        Task<User> LoginAsync(string username, string password);
    }
}
