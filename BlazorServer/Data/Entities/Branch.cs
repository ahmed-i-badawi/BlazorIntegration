using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities
{
    public class Branch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? Hash { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        //public int? MachineId { get; set; }
        public Machine Machine { get; set; }
    }
}
