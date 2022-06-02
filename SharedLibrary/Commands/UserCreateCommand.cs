using FluentValidation;
using SharedLibrary.Enums;
using SharedLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class UserCreateCommand
{
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public int UserType { get; set; }
    public int? SiteId { get; set; }
    public string? IntegratorHash { get; set; }

    public bool IsSendMail { get; set; }

}

public class UserCreateFM : UserCreateCommand
{
    public string Id { get; }

}

public class UserCreateFMValidator : AbstractValidator<UserCreateFM>
{
    public UserCreateFMValidator()
    {
        RuleFor(o => o.UserName).NotEmpty();
        RuleFor(o => o.FullName).NotEmpty();
        RuleFor(o => o.Password).Must(x => x.IsValidPassword()).WithMessage("Password Is Not Valid");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords must match");
        RuleFor(o => o.Email).Must(x => x.IsValidEmailAddress()).WithMessage("Email Is Not Valid");
        RuleFor(o => o.IsActive).NotEmpty();
        RuleFor(o => o.UserType).Must(e => e >= 0).WithMessage("UserType must not be empty");

        RuleFor(o => o.SiteId).Must(e => e > 0)
            .When(e => e.UserType == (int)UserType.Site)
            .WithMessage("Site must not be empty");

        RuleFor(o => o.IntegratorHash).Must(e => !string.IsNullOrWhiteSpace(e))
            .When(e => e.UserType == (int)UserType.Integrator)
            .WithMessage("Integrator must not be empty");
    }
}