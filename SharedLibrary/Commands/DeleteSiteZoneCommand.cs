using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class DeleteSiteZoneCommand
{
    public int ZoneId { get; set; }
    public int SiteId { get; set; }
}
