﻿using System;
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
        CreateMap<SiteDto , SiteCreateFM>()
             .ForMember(fm => fm.UserName, o => o.MapFrom(dto => dto.ApplicationUser.UserName))
             .ForMember(fm => fm.Email, o => o.MapFrom(dto => dto.ApplicationUser.Email));
        CreateMap<ZoneDto, ZoneCreateFM>();
        CreateMap<SiteZoneDto, SiteZoneCreateFM>();
    }
}
