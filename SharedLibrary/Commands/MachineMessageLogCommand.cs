using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class MachineMessageLogCommand
{
    public DateTime SentAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string Payload { get; set; }
    public int BrandId { get; set; }
    public int ZoneId { get; set; }
    public string? ConnectionId { get; set; }
    public string? MachineName { get; set; }
    public int? SiteId { get; set; }
    public string? SiteHash { get; set; }
}
