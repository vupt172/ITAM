using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.AppCore.Mappings
{
    public class ViTriTaiSanMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entity -> DTO (dùng khi đọc dữ liệu để hiển thị)
            config.NewConfig<ViTriTaiSan, ViTriTaiSanDto>()
                .Map(dest => dest.PhongBanName, src => src.PhongBan.Name);

            // DTO -> Entity (dùng khi Create/Update, gửi xuống Service)
            config.NewConfig<ViTriTaiSanDto, ViTriTaiSan>()
                .Ignore(dest => dest.PhongBan); // Tránh EF track nhầm navigation object rỗng
        }
    }
}
