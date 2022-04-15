using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities
{
    public class Branch
    {
        [Key]
        public Guid Id { get; }
        [NotMapped]
        public string Hash { get { return Id.ToString(); } }


        public string Name { get; set; }
        public string Address { get; set; }
        public string? Notes { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public Machine Machine { get; set; }
    }
}
