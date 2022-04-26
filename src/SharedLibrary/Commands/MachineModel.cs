using SharedLibrary.Enums;

namespace SharedLibrary.Commands;

public class MachineModel
{
    public string? SystemInfo { get; set; }
    public string? ConnectionId { get; set; }
    public int? SiteId { get; set; }
    public string? Hash { get; set; }
    public string? Notes { get; set; }
    public string? MachineName { get; set; }
}
