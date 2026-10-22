using ITAM.Domain.Entities.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class DieuChuyenVatTuChiTiet
    {
        public long Id { get; set; }
        public long DieuChuyenVatTuId { get; set; }
        public DieuChuyenVatTu DieuChuyenVatTu { get; set; } = default!;
        public long HangHoaId { get; set; }
        public HangHoa HangHoa { get; set; } = default!;
        public long ViTriChuyenDiId { get; set; }
        public ViTriTaiSan ViTriChuyenDi { get; set; } = default!;
        public long ViTriChuyenDenId { get; set; }
        public ViTriTaiSan ViTriChuyenDen { get; set; } = default!;
        public int SoLuong { get; set; }                          // TODO xác nhận: kiểu của VatTu.SoLuongTon
        public string? GhiChu { get; set; }
    }
}
