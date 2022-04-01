using Shared.Enums;

namespace BlazorServer.Data.Entities;

public class Machine
{
    public Machine()
    {
        MachineLogs = new List<MachineLog>();
    }
    public int Id { get; set; }
    public string? FingerPrint { get; set; }
    public string? Name { get; set; }
    public MachineStatus CurrentStatus { get; set; }
    //public MachineStatus CurrentStatus { 
    //    get {
    //        return MachineLogs?.MaxBy(i => i.OccurredAt).Status?? MachineStatus.Pending;
    //    } 
    //}
    public string? ConnectionId { get; set; }

    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public List<MachineLog>? MachineLogs { get; set; }
}
