﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SharedLibrary.Commands;
using SharedLibrary.Entities;

namespace BlazorServer.Mapping;
public class MapFromCommandToEntity : Profile
{
    public MapFromCommandToEntity()
    {
        CreateMap<IntegratorsRegistrationCreateCommand, Integrator>();
        CreateMap<BrandCreateCommand, Brand>();
        CreateMap<SiteCreateCommand, Site>();
        CreateMap<ZoneCreateCommand, Zone>();
    }
}
