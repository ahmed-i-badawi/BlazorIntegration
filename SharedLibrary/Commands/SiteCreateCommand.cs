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
    public string? ApplicationUserId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }

    public int MaxNumberOfMachines { get; set; }
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
        RuleFor(o => o.Email).NotEmpty();
        RuleFor(o => o.UserName).NotEmpty();
        RuleFor(o => o.Password).NotEmpty().When(e=>string.IsNullOrWhiteSpace(e.HashString));

        RuleFor(o => o.MaxNumberOfMachines).NotEmpty();
    }
}