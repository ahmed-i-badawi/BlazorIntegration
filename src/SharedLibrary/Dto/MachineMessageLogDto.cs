using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto;

public class MachineMessageLogDto
{
    public int Id { get; set; }
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
