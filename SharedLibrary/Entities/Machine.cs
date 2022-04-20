using SharedLibrary.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Entities;

public class Machine
{
    public Machine()
    {
        MachineLogs = new List<MachineLog>();
    }

    //[DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(1000)]
    public string? FingerPrint { get; set; }
    public string? Name { get; set; }
    public MachineStatus CurrentStatus { get; set; }
    //public MachineStatus CurrentStatus { 
    //    get {
    //        return MachineLogs?.MaxBy(i => i.OccurredAt).Status?? MachineStatus.Pending;
    //    } 
    //}
    //public string? ConnectionId { get; set; }

    public int SiteId { get; set; }
    public Site Site { get; set; }

    public List<MachineLog>? MachineLogs { get; set; }
}
