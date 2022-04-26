using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class IntegratorsRegistrationCreateCommand
{
    public string? Hash { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
}

public class IntegratorsRegistrationCreateFM : IntegratorsRegistrationCreateCommand
{
}

public class IntegratorsRegistrationCreateFMValidator : AbstractValidator<IntegratorsRegistrationCreateFM>
{
    public IntegratorsRegistrationCreateFMValidator()
    {
        RuleFor(o => o.Name).NotEmpty();
        RuleFor(o => o.Notes).NotEmpty();
    }
}