using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sem_1_lab_05_save_manager.Models
{
    public class SaveData
    {
        public string Character { get; set; }
        public string Map { get; set; }
        public Position Position { get; set; }
        public int Health { get; set; }
        public List<string> Inventory { get; set; }
    }
}
