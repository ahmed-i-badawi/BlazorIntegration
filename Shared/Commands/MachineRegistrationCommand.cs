﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Commands
{
    public class MachineRegistrationCommand
    {
        public string Hash { get; set; }
        public string MachineName { get; set; }
        public string Notes { get; set; }
        public string? ConnectionId { get; set; }
        public string? SystemInfo { get; set; }
    }
}
