using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto;

public class MachineDto
{
    public int BrandId { get; set; }
    public int SiteId { get; set; }
    public List<int>? ZoneIds { get; set; }
    public string Token { get; set; }
    public string? Name { get; set; }

}
