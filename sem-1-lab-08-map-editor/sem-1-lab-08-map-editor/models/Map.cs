using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sem_1_lab_08_map_editor.models
{
    public class Map
    {
        Cell[,] Field;
        Player Player;

        public Map(Cell[,] field, Player player)
        {
            Field = field;
            Player = player;
        }

        public Map()
        {
        }

        public static Map Load(string dir)
        {
            //
            if (!File.Exists(dir))
            {
                throw new FileNotFoundException();
            }
            string[] lines = File.ReadAllLines(dir);
            string sizeLine = lines[0];
            //
            if (!sizeLine.Contains('|'))
            {
                throw new FormatException();
            }
            int indexSeparator = sizeLine.IndexOf("|");
            int width = int.Parse(sizeLine.Substring(0, indexSeparator));
            int height = int.Parse(sizeLine.Substring(indexSeparator + 1, sizeLine.Length - (indexSeparator + 1)));
            Cell[,] newField = new Cell[height, width];
            Player? player = null;
            for (int i = 1; i < lines.Length; i++)
            {
                char[] line = lines[i].ToCharArray();
                for (int j = 0; j < line.Length; j++)
                {
                    switch (line[j])
                    {
                        case 'W':
                            newField[i - 1, j] = new Cell("WATER");
                            break;
                        case '#':
                            newField[i - 1, j] = new Cell("WALL");
                            break;
                        case '.':
                            newField[i - 1, j] = new Cell("GROUND");
                            break;
                        case '^':
                            newField[i - 1, j] = new Cell("LAVA");
                            break;
                        case 'P':
                            newField[i - 1, j] = new Cell("GROUND");
                            player = new Player("John", new Position(i - 1, j));
                            break;
                        default:
                            newField[i - 1, j] = new Cell("GROUND");
                            break;
                    }
                }
            }
            if (player == null)
            {
                throw new ArgumentNullException("Игрок на карте не был найден");
            }
            return new Map(newField, player.Value);
        }

        public static string[] Save(Map map)
        {
            Cell[,] mapField = map.GetMapField();
            Position playerPosition = map.GetPlayerPosition();

            int width = mapField.GetLength(1);
            int height = mapField.GetLength(0);
            string[] lines = new string[width + 1];
            lines[0] = $"{width}|{height}";
            for (int i = 0; i < mapField.GetLength(0); i++)
            {
                for (int j = 0; j < mapField.GetLength(1); j++)
                {
                    if (i == playerPosition.x && j == playerPosition.y)
                    {
                        lines[i + 1] += "P";
                        continue;
                    }
                    Cell cell = mapField[i, j];
                    switch (cell.GetType())
                    {
                        case "WATER":
                            lines[i + 1] += "W";
                            break;
                        case "WALL":
                            lines[i + 1] += "#";
                            break;
                        case "GROUND":
                            lines[i + 1] += ".";
                            break;
                        case "LAVA":
                            lines[i + 1] += "^";
                            break;
                        default:
                            lines[i + 1] += ".";
                            break;
                    }
                }
            }
            return lines;
        }

        public Map CreateSnapshot()
        {
            return this;
        }
        public void Restore(Map map)
        {
            this.Field = map.Field;
            this.Player = map.Player;
        }
        public void MovePlayer(Position newPosition)
        {
            //
            if (newPosition.x >= this.Field.GetLength(1) || newPosition.y >= this.Field.GetLength(0))
            {
                throw new IndexOutOfRangeException();
            }
            //
            if (Field[newPosition.y, newPosition.x].GetType() == "WALL")
            {
                throw new ArgumentException("Игрок не может занять клетку стены");
            }
            //
            if (Field[newPosition.x, newPosition.y].GetType() == "WATER")
            {
                throw new ArgumentException("Игрок утонул");
            }
            this.Player.Move(newPosition);
        }

        public void ChangeCell(Position position, string newType)
        {
            Position playerPosition = GetPlayerPosition();
            //
            if (position.x >= this.Field.GetLength(1) || position.y >= this.Field.GetLength(0))
            {
                throw new IndexOutOfRangeException();
            }
            //
            if (position.x == playerPosition.x && position.y == playerPosition.y &&
                (newType == "WALL" || newType == "WATER" || newType == "LAVA"))
            {
                throw new ArgumentException
                (
                    "Вы изменяет на клетку на тип, в котором не может находится игрок, " +
                    "что находится находиться на этой клетке"
                );
            }
            this.Field[position.y, position.x].ChangeType(newType);
        }


        public Position GetPlayerPosition()
        {
            return Player.GetPosition();
        }
        public Cell[,] GetMapField()
        {
            return Field;
        }
    }
}
