using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto;

public class SiteZoneDto
{
    public int SiteId { get; set; }
    public SiteDto Site { get; set; }

    public int ZoneId { get; set; }
    public ZoneDto Zone { get; set; }

    public string? Notes { get; set; }
}
