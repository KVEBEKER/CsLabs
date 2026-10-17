using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sem_1_lab_05_save_manager.models
{
    public class SaveLog
    {
        public DateTime makeAt { get; set; }
        public string name { get; set; }
        public string operation { get; set; }
    }
}
