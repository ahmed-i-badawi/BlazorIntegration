using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServer.Data.Entities
{
    public class SiteZone
    {
        public int SiteId { get; set; }
        public Site Site { get; set; }

        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
    }
}
