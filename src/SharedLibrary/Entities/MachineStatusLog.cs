using SharedLibrary.Enums;

namespace SharedLibrary.Entities;

public class MachineStatusLog
{
    public int Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public MachineStatus Status { get; set; }
    public string? ConnectionId { get; set; }
    public int MachineId { get; set; }
    public string MachineName { get; set; }
    public int SiteId { get; set; }
    public string? SiteUserId { get; set; }
    public string SiteHash { get; set; }
    public string SiteName { get; set; }
    public int BrandId { get; set; }
    public string BrandName { get; set; }
}
