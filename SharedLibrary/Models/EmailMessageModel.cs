using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class EmailMessageModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public EmailMessageModel(string to, string subject, string content, string from = null)
        {
            From = from;
            To = to;
            Subject = subject;
            Content = content;
        }
    }
}
