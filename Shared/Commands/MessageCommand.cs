using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Commands
{
    public class MessageCommand
    {
        public int BranchId { get; set; }
        public int BrandId { get; set; }
        public string Notes { get; set; }
        public int StatusId { get; set; }
    }
}
