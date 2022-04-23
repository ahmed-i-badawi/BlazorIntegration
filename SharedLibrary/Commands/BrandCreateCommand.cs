using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class BrandCreateCommand
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
}

public class BrandCreateFM : BrandCreateCommand
{
}

public class BrandCreateFMValidator : AbstractValidator<BrandCreateFM>
{
    public BrandCreateFMValidator()
    {
        RuleFor(o => o.Name).NotEmpty();
        RuleFor(o => o.Notes).NotEmpty();
    }
}