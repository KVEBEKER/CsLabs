using sem_1_lab_08_map_editor.models;
using System;
using System.IO;

namespace sem_1_lab_08_map_editor
{
    internal class Program
    {
        private static string _mapDir = "./maps/map.txt";
        private static string _backupDir = "./maps/backup.txt";
        private static Map _map = new Map();
        private static void PrintMap()
        {
            Cell[,] mapField = _map.GetMapField();
            Position playerPosition = _map.GetPlayerPosition();
            for (int i = 0; i < mapField.GetLength(0); i++)
            {
                for (int j = 0; j < mapField.GetLength(1); j++)
                {
                    if (i == playerPosition.x && j == playerPosition.y)
                    {
                        Console.Write("P");
                        continue;
                    }
                    Cell cell = mapField[i, j];
                    switch (cell.GetType())
                    {
                        case "WATER":
                            Console.Write("W");
                            break;
                        case "WALL":
                            Console.Write("#");
                            break;
                        case "GROUND":
                            Console.Write(".");
                            break;
                        default:
                            Console.Write(".");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            if (File.Exists(_mapDir))
            {
                try
                {
                    _map = Map.Load(_mapDir);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine("Файл не найден");
                    return;
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("Игрок на карте бэкапа не был найден");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Карта не корректна");
                    return;
                }
                PrintMap();

                bool cycle = true;
                while (cycle)
                {
                    // Появление сообщения меню
                    Console.Clear();
                    Console.WriteLine("Редактор карт");
                    Console.WriteLine("Введите цифру для выбора опции");
                    Console.WriteLine("");
                    Console.WriteLine("1. Изменить клетку");
                    Console.WriteLine("2. Переместить персонажа");
                    Console.WriteLine("3. Создать бэкап");
                    Console.WriteLine("4. Загрузить бэкап");
                    Console.WriteLine("5. Создать новую карту");
                    Console.WriteLine("0. Выйти из программы");
                    Console.WriteLine("");
                    Console.WriteLine("Карта:");
                    PrintMap();
                    string buffer = Console.ReadLine();
                    switch (buffer)
                    {
                        case "1":
                            ChangeCells();
                            break;
                        case "2":
                            MovePlayer();
                            break;
                        case "3":
                            CreateBackup();
                            break;
                        case "4":
                            LoadBackup();
                            break;
                        case "5":
                            CreateNewMap();
                            break;
                        case "0":
                            cycle = false;
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Карта не была найдена.");
            }
        }

        private static void CreateNewMap()
        {
            Console.Clear();
            //
            Console.WriteLine("Введите высоту карты");
            string bufferX = Console.ReadLine();
            int x = 0;
            if (!int.TryParse(bufferX, out x))
            {
                Console.WriteLine("Вы ввели не число");
                Console.ReadKey();
                return;
            }
            //
            Console.WriteLine("Введите ширину карты");
            string bufferY = Console.ReadLine();
            int y = 0;
            if (!int.TryParse(bufferY, out y))
            {
                Console.WriteLine("Вы ввели не число");
                Console.ReadKey();
                return;
            }
            if(x < 1 || y < 1)
            {
                Console.WriteLine("Нельзя построить карту. Карта слишком маленькая");
                return;
            }
            Cell[,] field = new Cell[x,y];
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    field[i, j] = new Cell("GROUND");
                }
            }
            Player player = new Player("Ivan", new Position(0,0));
            Map newMap = new Map(field, player);
            _map = newMap;
        }

        private static void LoadBackup()
        {
            Console.Clear();
            if (File.Exists(_backupDir))
            {
                try
                {
                    Map backupMap = Map.Load(_backupDir);
                    _map = backupMap;
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine("Файл не найден");
                    Console.ReadKey();
                    return;
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("Игрок на карте бэкапа не был найден");
                    Console.ReadKey();
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Карта бэкапа не корректна");
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                Console.WriteLine("Бэкап не был найден. Хотите создать Бэкап?");
                Console.WriteLine("1. Да");
                Console.WriteLine("2. Нет");
                string buffer = Console.ReadLine();
                switch (buffer)
                {
                    case "1":
                        CreateBackup();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void CreateBackup()
        {
            string[] lines = Map.Save(_map);
            File.WriteAllLines(_backupDir, lines);
            Console.WriteLine("Бэкап был создан");
            Console.ReadKey();
        }

        private static void MovePlayer()
        {
            Console.Clear();
            PrintMap();
            //
            Console.WriteLine("Введите на какую строку хотите переместить? (начинается с нуля)");
            string bufferX = Console.ReadLine();
            int x = 0;
            if (!int.TryParse(bufferX, out x))
            {
                Console.WriteLine("Вы ввели не число");
                Console.ReadKey();
                return;
            }
            //
            Console.WriteLine("Введите на какую столбец хотите переместить? (начинается с нуля)");
            string bufferY = Console.ReadLine();
            int y = 0;
            if (!int.TryParse(bufferY, out y))
            {
                Console.WriteLine("Вы ввели не число");
                Console.ReadKey();
                return;
            }
            //
            try
            {
                _map.MovePlayer(new Position(x, y));
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Игрок вышел за пределы карты");
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }
        }

        private static void ChangeCells()
        {
            Console.Clear();
            PrintMap();
            //
            Console.WriteLine("Введите на какой строке хотите изменить клетку? (начинается с нуля)");
            string bufferX = Console.ReadLine();
            int x = 0;
            if (!int.TryParse(bufferX, out x))
            {
                Console.WriteLine("Вы ввели не число");
                Console.ReadKey();
                return;
            }
            //
            Console.WriteLine("Введите на какую клетку хотите изменить? (начинается с нуля)");
            string bufferY = Console.ReadLine();
            int y = 0;
            if (!int.TryParse(bufferY, out y))
            {
                Console.WriteLine("Вы ввели не число");
                Console.ReadKey();
                return;
            }
            //
            string type = "GROUND";
            Console.WriteLine("На какой тип клетки вы хотите поменять?");
            Console.WriteLine("1. Земля");
            Console.WriteLine("2. Стена");
            Console.WriteLine("3. Лава");
            Console.WriteLine("4. Вода");
            string bufferChoice = Console.ReadLine();
            switch (bufferChoice)
            {
                case "1":
                    type = "GROUND";
                    break;
                case "2":
                    type = "WALL";
                    break;
                case "3":
                    type = "LAVA";
                    break;
                case "4":
                    type = "WATER";
                    break;
                default:
                    Console.WriteLine();
                    Console.ReadKey();
                    return;
                    break;
            }
            try
            {
                _map.ChangeCell(new Position(x, y), type);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Данной клетки не существует на карте");
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }
        }
    }
}
