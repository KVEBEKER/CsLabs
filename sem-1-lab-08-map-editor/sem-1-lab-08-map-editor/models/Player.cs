using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sem_1_lab_08_map_editor.models
{
    public struct Player
    {
        string Name;
        Position Position;
        public Player(string name, Position position)
        {
            this.Name = name;
            this.Position = position;
        }
        public void Move(Position newPosition)
        {
            this.Position = newPosition;
        }
        public Position GetPosition()
        {
            return this.Position;
        }
        public string GetName()
        {
            return this.Name;
        }
    }
}
