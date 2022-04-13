using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Shared.Commands;
using BlazorServer.Data.Entities;

namespace BlazorServer.Mapping;
public class MapFromCommandToEntity : Profile
{
    public MapFromCommandToEntity()
    {
        CreateMap<IntegratorsRegistrationCreateCommand, Integrator>();
    }
}
