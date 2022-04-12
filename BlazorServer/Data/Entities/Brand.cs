using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities;

public class Brand
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Branch>? Branches { get; set; }
}
