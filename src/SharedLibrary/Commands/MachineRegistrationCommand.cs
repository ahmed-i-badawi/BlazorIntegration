using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class MachineRegistrationCommand
{
    public int? BrandId { get; set; }
    public int? SiteId { get; set; }
    public List<int>? ZoneIds { get; set; }
    public string Hash { get; set; }
    public string MachineName { get; set; }
    public string Notes { get; set; }
    public string? ConnectionId { get; set; }
    public string? SystemInfo { get; set; }

}

public class MachineRegistrationFM : MachineRegistrationCommand
{
}

public class MachineRegistrationFMValidator : AbstractValidator<MachineRegistrationFM>
{
    public MachineRegistrationFMValidator()
    {
        RuleFor(o => o.Hash).NotEmpty();
        RuleFor(o => o.MachineName).NotEmpty();
        //RuleFor(o => o.Notes).NotEmpty();
    }
}
