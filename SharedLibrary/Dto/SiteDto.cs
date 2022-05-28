using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto;

public class SiteDto
{
    public SiteDto ()
    {
        //Zones = new List<ZoneDto> ();
    }
    public int Id { get; set; }
    public string HashString { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Notes { get; set; }
    public int NumberOfZones { get; set; }
    public int ActualNumberOfMachines { get; set; }
    public int MaxNumberOfMachines { get; set; }
    public bool IsCallCenter { get; set; }

    public string? ApplicationUserId { get; set; }
    public ApplicationUserDto ApplicationUser { get; set; }

    public List<ZoneDto>? Zones { get; set; }
    public int BrandId { get; set; }
    public BrandDto Brand { get; set; }
    public List<MachineDto> Machines { get; set; }
}
