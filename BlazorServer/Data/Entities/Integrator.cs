using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities;

public class Integrator
{
    public Integrator()
    {

    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Hash { get; set; }
}
