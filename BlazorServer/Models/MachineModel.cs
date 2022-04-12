using Shared.Enums;

namespace BlazorServer.Data.Entities;

public class MachineModel
{
    public string? sysInfo { get; set; }
    public string? ConnectionId { get; set; }
    public string? Hash { get; set; }
    public string? Notes { get; set; }
    public string? MachineName { get; set; }
}
