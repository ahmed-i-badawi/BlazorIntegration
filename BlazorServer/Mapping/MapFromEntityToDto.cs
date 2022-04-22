using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SharedLibrary.Dto;
using SharedLibrary.Entities;

namespace BlazorServer.Mapping;
public class MapFromEntityToDto : Profile
{
    public MapFromEntityToDto()
    {
        CreateMap<Integrator, IntegratorsDto>();
        CreateMap<Brand, BrandDto>();
        CreateMap<Site, SiteDto>();
        CreateMap<Zone, ZoneDto>();
        CreateMap<SiteZone, SiteZoneDto>();
        CreateMap<Site, SiteDto>();
        CreateMap<Machine, MachineDto>();
        CreateMap<MachineStatusLog, MachineStatusLogDto>();
        CreateMap<MachineMessageLog, MachineMessageLogDto>();
    }
}
