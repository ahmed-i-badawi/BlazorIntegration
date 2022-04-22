using FluentValidation;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto;

public class MachineStatusLogDto
{
    public int Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public MachineStatus Status { get; set; }
    public string? ConnectionId { get; set; }
    public int MachineId { get; set; }
    public string MachineName { get; set; }
    public int SiteId { get; set; }
    public string SiteHash { get; set; }
    public string SiteName { get; set; }
    public int BrandId { get; set; }
    public string BrandName { get; set; }

}
