using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities
{
    public class Site
    {
        public int Id { get; }
        public Guid Hash { get; }
        [NotMapped]
        public string HashString { get { return Hash.ToString(); } }

        public string Name { get; set; }
        public string Address { get; set; }
        public string? Notes { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public List<SiteZone>? SiteZones { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int NumberOfZones
        {
            get { return SiteZones?.Count ?? 0; }
            private set { /* needed for EF */ }
        }
        public bool IsCallCenter { get; set; }
        public Machine Machine { get; set; }
    }
}
