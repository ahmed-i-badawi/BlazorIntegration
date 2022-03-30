using Shared.Enums;

namespace BlazorServer.Data.Entities;

public class Machine
{
    public int Id { get; set; }
    public string? FingerPrint { get; set; }
    public string Name { get; set; }
    public DateTime DateAdded { get; set; }
    public MachineStatus CurrentStatus { get; set; }
    public string? ConnectionId { get; set; }

    public int BrandId { get; set; }
    public Brand Brand { get; set; }

    public List<MachineLog>? MachineLogs { get; set; }
}
