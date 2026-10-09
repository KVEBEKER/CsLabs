using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sem_1_lab_04_event_log.Models
{
    public class LogEntry
    {
        public DateTime Timestamp;
        public string Level;
        public string Category;
        public string Message;
    }
}
