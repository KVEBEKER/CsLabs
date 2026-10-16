using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sem_1_lab_05_save_manager.Models
{
    public class SaveEntry
    {
        public string name {  get; set; }
        public string file { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
