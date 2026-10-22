using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class LichSuBienDongVatTu
    {
        public long Id { get; set; }
        public long HangHoaId { get; set; }
        public long ViTriTaiSanId { get; set; }
        public LoaiBienDongVatTu Loai { get; set; }
        public int SoLuong { get; set; }                          // luôn dương
        public int TonSau { get; set; }
        public long? DieuChuyenVatTuChiTietId { get; set; }       // liên kết ngược nullable, giống LichSuDieuChuyenTaiSan
        public long NguoiDuyetId { get; set; }
        public DateTime NgayDuyet { get; set; }
        public string? GhiChu { get; set; }
    }
}
