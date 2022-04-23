using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class SiteZoneCreateCommand
{
    public int ZoneId { get; set; }
    public int SiteId { get; set; }
    public string Notes { get; set; }

}

public class SiteZoneCreateFM : SiteZoneCreateCommand
{
}

public class SiteZoneCreateFMValidator : AbstractValidator<SiteZoneCreateFM>
{
    public SiteZoneCreateFMValidator()
    {
        //RuleFor(o => o.BrandId).NotEmpty();
    }
}