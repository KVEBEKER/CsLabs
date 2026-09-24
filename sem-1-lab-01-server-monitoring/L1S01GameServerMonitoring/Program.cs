using L1S01GameServerMonitoring.Model;
using System.Diagnostics;
using System.Management;
using System.Text.Json;

namespace L1S01GameServerMonitoring
{
    internal class Program
    {
        const int maxPlayers = 16;
        const string jsonPath = @"C:\GServer";
        const string jsonCharPath = @"\SaveChar";
        const string jsonStatePath = @"\Logs";

        /// <summary>
        /// Выводит в консоль о состоянии
        /// </summary>
        /// <param name="state">Выводимое состояние</param>
        static void RevealServerState(ServerState state)
        {
            Console.WriteLine("=-=-=-=-=-=-=-=-=");
            Console.WriteLine($"Операционная система: {state.Oc}");
            Console.WriteLine($"Доступные процессы: {state.AvailableProcessorCount}");
            Console.WriteLine($"Оперативная память: {state.FreeRam}/{state.TotalRam}");
            Console.WriteLine($"Дисковая память: {state.TotalMemory}");
            Console.WriteLine($"Количество игроков: {state.PlayerCount}/{state.MaxPlayerCount}");
            Console.WriteLine("=-=-=-=-=-=-=-=-=");
        }

        /// <summary>
        /// Прочитать предыдущую сессию
        /// </summary>
        static ServerState? ReadPastSession()
        {
            if (!Directory.Exists(jsonPath + jsonStatePath))
            {
                return null;
            }

            FileInfo? newestFile = null;

            foreach (var file in new DirectoryInfo(jsonPath + jsonStatePath).EnumerateFiles(
                "*.json",
                SearchOption.TopDirectoryOnly
                ))
            {
                if (newestFile == null || file.LastWriteTimeUtc > newestFile.LastWriteTimeUtc)
                    newestFile = file;
            }

            if (newestFile == null)
            {
                return null;
            }

            string? jsonString = File.ReadAllText(newestFile.FullName);
            ServerState? state = JsonSerializer.Deserialize<ServerState>(jsonString);
            if (state != null)
            {
                return state;
            }
            return null;
        }

        /// <summary>
        /// Сохраняет файлы сессии в json
        /// </summary>
        /// <param name="state">Сохраняемое состояние</param>
        static void SaveFileAboutSession(ServerState state)
        {
            string fileName = $"SavedSession{DateTime.Now.ToShortDateString()}.json";

            Directory.CreateDirectory(@$"{jsonPath}{jsonStatePath}");

            string? jsonString = JsonSerializer.Serialize(state);
            File.WriteAllText(@$"{jsonPath}{jsonStatePath}/{fileName}", jsonString);
        }

        /// <summary>
        /// Читает (если есть) файл с персонажами
        /// </summary>
        /// <returns>Возвращает лист сохранённых игроков</returns>
        static List<Player> ReadSavedPlayersJson()
        {
            if (!Directory.Exists(jsonPath + jsonCharPath))
            {
                return new List<Player>();
            }

            string? jsonString = File.ReadAllText(jsonPath + jsonCharPath);
            List<Player>? players = JsonSerializer.Deserialize<List<Player>>(jsonString);
            if (players != null)
            {
                Console.WriteLine("Персонажи успешно загружены");
                return players;
            }
            Console.WriteLine("Персонажи не найдены. Выдаю пустой лист");
            return new List<Player>();
        }

        /// <summary>
        /// Сохраняет персонажей в виде Json-файла
        /// </summary>
        /// <param name="players"></param>
        static void SaveSavedPlayersJson(List<Player> players)
        {
            string fileName = "players.json";

            Directory.CreateDirectory(@$"{jsonPath}{jsonCharPath}");

            string? jsonString = JsonSerializer.Serialize(players);
            File.WriteAllText(@$"{jsonPath}{jsonCharPath}\{fileName}", jsonString);
        }
        static void Main(string[] args)
        {
            //Получение системных данных
            string oc = Environment.OSVersion.ToString();
            int processorCount = Environment.ProcessorCount;
            ulong ramTotal = 0;
            using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    ramTotal = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                    break;
                }
            }
            float totalMemoryMb = ramTotal / 1024 / 1024;
            PerformanceCounter ramFree = new PerformanceCounter("Memory", "Available MBytes");
            float ramFreeFloat = ramFree.NextValue();
            PerformanceCounter memoryTotal = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            float memoryTotalFloat = ramFree.NextValue();
            //Получение информации о игроках
            List<Player> savedPlayers = ReadSavedPlayersJson();
            int playerCount = savedPlayers.Count;
            //Составление состояние сервера
            ServerState s = new ServerState(
                oc,
                processorCount,
                ramTotal,
                ramFreeFloat,
                memoryTotalFloat,
                maxPlayers,
                playerCount,
                savedPlayers
            );
            Console.Clear();
            Console.WriteLine("Состояние сервера составлено");
            Thread.Sleep(1000);
            Console.Clear();
            //Цикличное меню
            bool cycle = true;
            while (cycle)
            {
                // Появление сообщения меню
                Console.Clear();
                Console.WriteLine("Менеджер профиля игрока");
                Console.WriteLine("");
                Console.WriteLine("1. Показать текущее состояние");
                Console.WriteLine("2. Показать предыдущие состояние");
                Console.WriteLine("0. Выйти из программы");
                // Ввод выбора
                string option = Console.ReadLine();
                // Обработка ввода и вывод
                Console.Clear();
                switch (option)
                {
                    // Показывает текущее состояние сервера
                    case "1":
                        RevealServerState(s);
                        Console.WriteLine("Нажмите чтобы продолжить");
                        Console.ReadKey();
                        break;
                    // Вывод предыдущего состояние сервера
                    case "2":
                        ServerState? pastServerState = ReadPastSession();
                        if (pastServerState != null)
                        {
                            RevealServerState(pastServerState);
                            Console.WriteLine("Нажмите чтобы продолжить");
                            Console.ReadKey();
                        }
                        Console.WriteLine("Предыдущая сессия не найдена. Нажмите чтобы продолжить");
                        Console.ReadKey();
                        break;
                    // Выход из программы
                    case "0":
                        //Сохранение игроков и состояния
                        SaveFileAboutSession(s);
                        SaveSavedPlayersJson(savedPlayers);
                        cycle = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
