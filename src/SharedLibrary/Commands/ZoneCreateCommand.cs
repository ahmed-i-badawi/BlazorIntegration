using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class ZoneCreateCommand
{
    public int Id { get; }
    public string Name { get; set; }
    public string? Notes { get; set; }
}

public class ZoneCreateFM : ZoneCreateCommand
{

}

public class ZoneCreateFMValidator : AbstractValidator<ZoneCreateFM>
{
    public ZoneCreateFMValidator()
    {
        RuleFor(o => o.Name).NotEmpty();
        RuleFor(o => o.Notes).NotEmpty();
    }
}