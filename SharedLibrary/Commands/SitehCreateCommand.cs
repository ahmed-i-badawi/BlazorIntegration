using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class SiteCreateCommand
{
    public int Id { get; set; }
    public string? HashString { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Notes { get; set; }
    public int BrandId { get; set; }
}

public class SiteCreateFM : SiteCreateCommand
{
    public string BrandName { get; set; }
}

public class SiteCreateFMValidator : AbstractValidator<SiteCreateFM>
{
    public SiteCreateFMValidator()
    {
        RuleFor(o => o.Name).NotEmpty();
        RuleFor(o => o.Address).NotEmpty();
        RuleFor(o => o.Notes).NotEmpty();
        RuleFor(o => o.BrandId).NotEmpty();
    }
}