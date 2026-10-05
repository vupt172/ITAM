using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{

        public interface IPhieuThatLacService
        {
            /// <summary>
            /// Lưu nháp phiếu (tạo mới nếu dto.Id null/0, ngược lại sửa phiếu PENDING). Trả về Id phiếu.
            /// </summary>
            Task<long> LuuPhieuAsync(SavePhieuThatLacDto dto);

            /// <summary>
            /// Duyệt phiếu: chuyển tài sản sang LOST, đổi vị trí về Kho Thất Lạc, ghi lịch sử.
            /// </summary>
            Task DuyetPhieuAsync(long phieuThatLacId, long nguoiDuyetId);

            /// <summary>Từ chối phiếu PENDING, tài sản không đổi gì.</summary>
            Task TuChoiPhieuAsync(long phieuThatLacId, long nguoiDuyetId);

            /// <summary>Xóa phiếu, chỉ cho phép khi PENDING.</summary>
            Task DeleteAsync(long phieuThatLacId);

            Task<PhieuThatLac?> GetByIdAsync(long phieuThatLacId);

            Task<List<PhieuThatLac>> TimKiemAsync(PhieuThatLacSearchDto dto);
        }
    }

