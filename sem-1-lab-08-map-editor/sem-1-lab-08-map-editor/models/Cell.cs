using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sem_1_lab_08_map_editor.models
{
    /// <summary>
    /// Клетки для карты. Его типы:
    /// '#' - "WALL"
    /// '.' - "GROUND"
    /// 'W' - "WATER"
    /// '^' - "LAVA"
    /// </summary>
    public class Cell
    {
        string type = "GROUND";

        public Cell(string type) 
        {
            this.type = type;
        }
        public string GetType()
        {
            return type;
        }

        public void ChangeType(string newType)
        {
            this.type = newType;
        }
    }
}
