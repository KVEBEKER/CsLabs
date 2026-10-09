using sem_1_lab_04_event_log.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace sem_1_lab_04_event_log
{
    public class Program
    {
        public static LogEntry[] ParseLog(string[] lines)
        {
            //Создаем массив
            LogEntry[] entries = new LogEntry[lines.Length];
            //Заполняем массив
            for (int i = 0; i < lines.Length; i++)
            {
                //Нахождение раздела строк
                int indexTime1 = lines[i].IndexOf(' ');
                int indexTime2 = lines[i].IndexOf(' ', indexTime1 + 1);
                int indexOpenLevel = lines[i].IndexOf('[');
                int indexCloseLevel = lines[i].IndexOf(']');
                int indexOpenCategory = lines[i].IndexOf('[', indexOpenLevel + 1);
                int indexCloseCategory = lines[i].IndexOf(']', indexCloseLevel + 1);
                int indexMessage = lines[i].IndexOf(' ', indexTime2 + 1);
                //Подстроки
                string timeString = lines[i].Substring(0, indexTime2);
                string levelString = lines[i].Substring(indexOpenLevel + 1, indexCloseLevel - indexOpenLevel - 1);
                string categoryString = lines[i].Substring(indexOpenCategory + 1, indexCloseCategory - indexOpenCategory - 1);
                string message = lines[i].Substring(indexMessage + 1, lines[i].Length - indexMessage - 1);
                //Перевод в данные
                DateTime logTimestamp = DateTime.Parse(timeString);
                //Заполняем класс лога и передаем в массив
                LogEntry log = new LogEntry();
                log.Timestamp = logTimestamp;
                log.Level = levelString;
                log.Category = categoryString;
                log.Message = message;
                entries[i] = log;
            }
            return entries;
        }
        public static LogEntry[] FilterByDate(LogEntry[] entries, DateTime date)
        {
            int count = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].Timestamp == date)
                {
                    count++;
                }
            }
            LogEntry[] filterEntries = new LogEntry[count];
            int pivot = 0;
            for (int i = 0; i < filterEntries.Length; i++)
            {
                for (int y = pivot; y < entries.Length; y++)
                {
                    if (entries[y].Timestamp == date)
                    {
                        filterEntries[i] = entries[y];
                        pivot = y + 1;
                        break;
                    }
                }
            }
            return filterEntries;
        }

        public static LogEntry[] FilterByDate(LogEntry[] entries, DateOnly date)
        {
            int count = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                if
                    (
                    entries[i].Timestamp.Year == date.Year &&
                    entries[i].Timestamp.Month == date.Month &&
                    entries[i].Timestamp.Day == date.Day
                    )
                {
                    count++;
                }
            }
            LogEntry[] filterEntries = new LogEntry[count];
            int pivot = 0;
            for (int i = 0; i < filterEntries.Length; i++)
            {
                for (int y = pivot; y < entries.Length; y++)
                {
                    if
                        (
                        entries[y].Timestamp.Year == date.Year &&
                        entries[y].Timestamp.Month == date.Month &&
                        entries[y].Timestamp.Day == date.Day
                        )
                    {
                        filterEntries[i] = entries[y];
                        pivot = y + 1;
                        break;
                    }
                }
            }
            return filterEntries;
        }

        public static LogEntry[] FilterByLevel(LogEntry[] entries, string level)
        {
            int count = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].Level == level)
                {
                    count++;
                }
            }
            LogEntry[] filterEntries = new LogEntry[count];
            int pivot = 0;
            for (int i = 0; i < filterEntries.Length; i++)
            {
                for (int y = pivot; y < entries.Length; y++)
                {
                    if (entries[y].Level == level)
                    {
                        filterEntries[i] = entries[y];
                        pivot = y + 1;
                        break;
                    }
                }
            }
            return filterEntries;
        }

        public static LogEntry[] FilterByCategory(LogEntry[] entries, string category)
        {
            int count = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].Category == category)
                {
                    count++;
                }
            }
            LogEntry[] filterEntries = new LogEntry[count];
            int pivot = 0;
            for (int i = 0; i < filterEntries.Length; i++)
            {
                for (int y = pivot; y < entries.Length; y++)
                {
                    if (entries[y].Category == category)
                    {
                        filterEntries[i] = entries[y];
                        pivot = y + 1;
                        break;
                    }
                }
            }
            return filterEntries;
        }
        public static LogEntry[] Search(LogEntry[] entries, string text)
        {
            int count = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].Message.ToLower().Contains(text.ToLower()))
                {
                    count++;
                }
            }
            LogEntry[] filterEntries = new LogEntry[count];
            int pivot = 0;
            for (int i = 0; i < filterEntries.Length; i++)
            {
                for (int y = pivot; y < entries.Length; y++)
                {
                    if (entries[y].Message.ToLower().Contains(text.ToLower()))
                    {
                        filterEntries[i] = entries[y];
                        pivot = y + 1;
                        break;
                    }
                }
            }
            return filterEntries;
        }

        public static int CountByLevel(LogEntry[] entries, string level)
        {
            int count = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].Level == level)
                {
                    count++;
                }
            }
            return count;
        }

        public static string GetServerStatus(LogEntry[] entries)
        {
            bool hasError = false;
            //Проверяем строки лога
            for (int i = 0; i < entries.Length; i++)
            {
                //Если есть ошибка - отмечаем
                if (entries[i].Level == "Error")
                {
                    hasError = true;
                }
                //Если есть критическая ошибка - Сразу выводим
                if (entries[i].Level == "Fatal" && entries[i].Category == "Server")
                {
                    return "КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен";
                }
            }
            //Если были найдены ошибки при просмотре лока - выводим
            if (hasError)
            {
                return "Есть ошибки: требуется проверка";
            }
            //Выводим если не нашли ошибок
            return "Сервер работает штатно";
        }

        static void WriteEntries(LogEntry[] logEntries)
        {
            foreach (LogEntry entry in logEntries)
            {
                Console.WriteLine($"{entry.Timestamp.ToString()} [{entry.Level}][{entry.Category}] {entry.Message}");
            }
        }


        static void Main(string[] args)
        {
            if (File.Exists("event_server.log"))
            {
                string[] lines = File.ReadAllLines("event_server.log");
                LogEntry[] entries = Program.ParseLog(lines);
                bool cycle = true;
                while (cycle)
                {
                    int option = -1;
                    int filterOption = -1;
                    Console.Clear();
                    Console.WriteLine("Введите номер варианта:");
                    Console.WriteLine("1. Вывести сообщения по уровню");
                    Console.WriteLine("2. Вывести сообщения по категории");
                    Console.WriteLine("3. Найти строки по сообщению");
                    Console.WriteLine("4. Статус сервера");
                    Console.WriteLine("0. Выход из программы");
                    if (int.TryParse(Console.ReadLine(), out option))
                    {
                        switch (option)
                        {
                            //Фильтр по уровню
                            case 1:
                                Console.Clear();
                                Console.WriteLine("Введите номер варианта:");
                                Console.WriteLine("1. Сообщения");
                                Console.WriteLine("2. Предупреждения");
                                Console.WriteLine("3. Ошибка");
                                Console.WriteLine("4. Фатальная ошибка");
                                if (int.TryParse(Console.ReadLine(), out filterOption))
                                {
                                    switch (filterOption)
                                    {
                                        case 1:
                                            WriteEntries(FilterByLevel(entries, "Info"));
                                            break;
                                        case 2:
                                            WriteEntries(FilterByCategory(entries, "Warning"));
                                            break;
                                        case 3:
                                            WriteEntries(FilterByCategory(entries, "Error"));
                                            break;
                                        case 4:
                                            WriteEntries(FilterByCategory(entries, "Fatal"));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            //Фильтр по категории
                            case 2:
                                Console.Clear();
                                Console.WriteLine("Введите номер варианта:");
                                Console.WriteLine("1. Сервер");
                                Console.WriteLine("2. Пользователь");
                                Console.WriteLine("3. Клан");
                                Console.WriteLine("4. Событие");
                                Console.WriteLine("5. НПС");
                                Console.WriteLine("6. Предметы");
                                Console.WriteLine("7. Бой");
                                Console.WriteLine("8. Награды");
                                Console.WriteLine("9. Статистика");
                                Console.WriteLine("0. База данных");
                                if (int.TryParse(Console.ReadLine(), out filterOption))
                                {
                                    switch (filterOption)
                                    {
                                        case 1:
                                            WriteEntries(FilterByCategory(entries, "Server"));
                                            break;
                                        case 2:
                                            WriteEntries(FilterByCategory(entries, "User"));
                                            break;
                                        case 3:
                                            WriteEntries(FilterByCategory(entries, "Clan"));
                                            break;
                                        case 4:
                                            WriteEntries(FilterByCategory(entries, "Event"));
                                            break;
                                        case 5:
                                            WriteEntries(FilterByCategory(entries, "NPC"));
                                            break;
                                        case 6:
                                            WriteEntries(FilterByCategory(entries, "Loot"));
                                            break;
                                        case 7:
                                            WriteEntries(FilterByCategory(entries, "Combat"));
                                            break;
                                        case 8:
                                            WriteEntries(FilterByCategory(entries, "Reward"));
                                            break;
                                        case 9:
                                            WriteEntries(FilterByCategory(entries, "Statistics"));
                                            break;
                                        case 0:
                                            WriteEntries(FilterByCategory(entries, "Database"));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            //Фильтр по сообщению
                            case 3:
                                Console.Clear();
                                string searchMessage = Console.ReadLine();
                                WriteEntries(Search(entries, searchMessage));
                                break;
                            //Статус работы сервера
                            case 4:
                                Console.Clear();
                                Console.WriteLine(GetServerStatus(entries));
                                break;
                            //Выход из программы
                            case 0:
                                cycle = false;
                                break;
                            //Обычный случай - Продолжение работы
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Файл не был найден.");
            }
        }
    }
}
