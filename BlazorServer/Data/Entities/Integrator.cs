using SharedLibrary.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities;

public class Integrator
{
    public Integrator()
    {

    }

    [Key]
    public Guid Id { get; }
    [NotMapped]
    public string Hash { get { return Id.ToString(); } }

    public string? Name { get; set; }
    public string? Notes { get; set; }
}
