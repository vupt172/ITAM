using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities.Catalogs;
using Mapster;

namespace ITAM.AppCore.Mappings
{
    public class HangHoaMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<HangHoa, HangHoaDto>()
                .Map(dest => dest.DMTaiSanName, src => src.DMTaiSan.Name);

            config.NewConfig<HangHoaDto, HangHoa>()
                .Ignore(dest => dest.DMTaiSan);
        }
    }
}
