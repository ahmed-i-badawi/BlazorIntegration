using SharedLibrary.Enums;

namespace BlazorServer.Data.Entities;

public class MachineLog
{
    public int Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public MachineStatus Status { get; set; }
    public string? ConnectionId { get; set; }
    public int MachineId { get; set; }
    public Machine Machine { get; set; }
}
