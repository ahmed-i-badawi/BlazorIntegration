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

    public List<ZoneDto>? Zones { get; set; }
    public int BrandId { get; set; }
    public BrandDto Brand { get; set; }
    public MachineDto Machine { get; set; }
}
