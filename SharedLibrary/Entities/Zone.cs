using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Entities;

public class Zone : AuditableEntity
{
    public int Id { get; }
    public string Name { get; set; }
    public string? Notes { get; set; }

    public List<SiteZone>? SiteZones { get; set; }
}
