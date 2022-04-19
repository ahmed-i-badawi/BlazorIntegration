using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SharedLibrary.Commands;
using SharedLibrary.Dto;

namespace BlazorServer.Mapping;
public class MapFromDtoToFM : Profile
{
    public MapFromDtoToFM()
    {
        CreateMap<IntegratorsDto , IntegratorsRegistrationCreateFM>();
        CreateMap<BrandDto , BrandCreateFM>();
        CreateMap<SiteDto , SiteCreateFM>();
        CreateMap<ZoneDto, ZoneCreateFM>();
        CreateMap<SiteZoneDto, SiteZoneCreateFM>();
    }
}
