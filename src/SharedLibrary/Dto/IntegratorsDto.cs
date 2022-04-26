using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dto;

public class IntegratorsDto
{
    public string Hash { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
}
