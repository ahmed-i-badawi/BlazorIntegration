using SharedLibrary.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Entities;

public class Machine : AuditableEntity
{
    public int Id { get; set; }
    [MaxLength(1000)]
    public string? FingerPrint { get; set; }
    public string? Name { get; set; }
    public MachineStatus CurrentStatus { get; set; }
    public int SiteId { get; set; }
    public Site Site { get; set; }
}
