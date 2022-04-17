using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities
{
    public class Site
    {
        [Key]
        public int Id { get; }
        public Guid Hash { get; }
        [NotMapped]
        public string HashString { get { return Hash.ToString(); } }

        public string Name { get; set; }
        public string Address { get; set; }
        public string? Notes { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public Machine Machine { get; set; }
    }
}
