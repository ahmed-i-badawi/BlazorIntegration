using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Entities;

public class Site : AuditableEntity
{
    public int Id { get; }
    public Guid Hash { get; set; }
    [NotMapped]
    public string HashString { get { return Hash.ToString(); } }

    public string Name { get; set; }
    public string Address { get; set; }
    public string? Notes { get; set; }

    public int BrandId { get; set; }
    public Brand Brand { get; set; }

    public List<SiteZone>? SiteZones { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int? NumberOfZones
    {
        get { return SiteZones?.Count ?? 0; }
        private set { /* needed for EF */ }
    }
    public bool IsCallCenter { get; set; }
    //toDo list of machines
    public Machine Machine { get; set; }

    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
}
