using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sem_1_lab_08_map_editor.models
{
    /// <summary>
    /// Класс карты.
    /// </summary>
    public class Map
    {
        //Собственно карта.
        Cell[,] Field;
        //Игрок, на которой ней находиться.
        Player Player;

        public Map(Cell[,] field, Player player)
        {
            Field = field;
            Player = player;
        }

        public Map()
        {
        }

        /// <summary>
        /// Достает из файла строки и форматирует их в карту.
        /// </summary>
        /// <param name="dir">Путь к файлу.</param>
        /// <returns>Карту.</returns>
        /// <exception cref="FileNotFoundException">Файл не найден.</exception>
        /// <exception cref="FormatException">Файл не корректен и из него нельзя сделать карту.</exception>
        /// <exception cref="ArgumentNullException">Игрока на карте файле нету.</exception>
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

        /// <summary>
        /// Переводит карту в строки для последующего сохранения через File.WriteAllLines().
        /// </summary>
        /// <param name="map">Карту, которую хотите сохранить.</param>
        /// <returns>Строки сформатированной карты.</returns>
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

        /// <summary>
        /// Создает копию карты
        /// </summary>
        /// <returns>Карта.</returns>
        public Map CreateSnapshot()
        {
            return this;
        }

        /// <summary>
        /// Перезаписывает карту.
        /// </summary>
        /// <param name="map">Карта с которой переписывают карту.</param>
        public void Restore(Map map)
        {
            this.Field = map.Field;
            this.Player = map.Player;
        }

        /// <summary>
        /// Перемещает игрока на новую позицию в поле.
        /// </summary>
        /// <param name="newPosition">Новая позиция игрока.</param>
        /// <exception cref="IndexOutOfRangeException">Игрок переместился за границу карты.</exception>
        /// <exception cref="ArgumentException">Игрок переместился на клетку препятствия.</exception>
        public void MovePlayer(Position newPosition)
        {
            //Проверяем не выходит ли за границы карты.
            if (newPosition.x >= this.Field.GetLength(1) || newPosition.y >= this.Field.GetLength(0))
            {
                throw new IndexOutOfRangeException();
            }
            if (newPosition.x < 0 || newPosition.y < 0)
            {
                throw new IndexOutOfRangeException();
            }
            //Проверяем не заходит ли игрок за недоступные клетки.
            //Стена
            if (Field[newPosition.y, newPosition.x].GetType() == "WALL")
            {
                throw new ArgumentException("Игрок не может занять клетку стены");
            }
            //Вода
            if (Field[newPosition.x, newPosition.y].GetType() == "WATER")
            {
                throw new ArgumentException("Игрок утонул");
            }
            //Лава
            if (Field[newPosition.x, newPosition.y].GetType() == "LAVA")
            {
                throw new ArgumentException("Игрок решил поплавать в лаве");
            }
            //После всех проверок перемещаем игрока.
            this.Player.Move(newPosition);
        }


        /// <summary>
        /// Меняет тип клетки на карте.
        /// </summary>
        /// <param name="position">Позиция клетки, которую хотим поменять..</param>
        /// <param name="newType">Тип клетки, на которую мы хотим поменять.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void ChangeCell(Position position, string newType)
        {
            //Получаем позицию игрока для проверки исключения.
            Position playerPosition = GetPlayerPosition();
            //Проверяем принадлежит координата клетки карте.
            if (position.x >= this.Field.GetLength(1) || position.y >= this.Field.GetLength(0))
            {
                throw new IndexOutOfRangeException();
            }
            if (position.x < 0 || position.y < 0)
            {
                throw new IndexOutOfRangeException();
            }
            //Проверяем не меняет ли клетку, на которой находится игрок на тип клетки препятствия.
            if (position.x == playerPosition.x && position.y == playerPosition.y &&
                (newType == "WALL" || newType == "WATER" || newType == "LAVA"))
            {
                throw new ArgumentException
                (
                    "Вы изменяет на клетку на тип, в котором не может находится игрок, " +
                    "что находится находиться на этой клетке"
                );
            }
            //После всех проверок изменяем клетку.
            this.Field[position.y, position.x].ChangeType(newType);
        }

        /// <summary>
        /// Получаем позицию игрока.
        /// </summary>
        /// <returns>Позицию игрока.</returns>
        public Position GetPlayerPosition()
        {
            return Player.GetPosition();
        }
        /// <summary>
        /// Получаем поле карты.
        /// </summary>
        /// <returns>Поле карты.</returns>
        public Cell[,] GetMapField()
        {
            return Field;
        }
    }
}
