﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Commands;

public class SubmitMachineCommand
{
    public string SystemInfo { get; set; }
    public string ConnectionId { get; set; } = "";
}
