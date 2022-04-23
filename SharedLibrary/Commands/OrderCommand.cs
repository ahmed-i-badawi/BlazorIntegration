using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class OrderCommand
{
    public int BrandId { get; set; }
    public int ZoneId { get; set; }
    public string? Notes { get; set; }
}
