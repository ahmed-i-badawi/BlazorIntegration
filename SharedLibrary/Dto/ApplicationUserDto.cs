using FluentValidation;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto;

public class ApplicationUserDto
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public UserType UserType { get; set; }
}
