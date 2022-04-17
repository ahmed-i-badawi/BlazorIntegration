using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities;

public class Brand
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Notes { get; set; }

    public List<Site>? Sites { get; set; }
}
