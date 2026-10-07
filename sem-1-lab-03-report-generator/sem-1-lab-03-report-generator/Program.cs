using System.Runtime.InteropServices.Marshalling;

namespace sem_1_lab_03_report_generator
{
    public class Program
    {
        static (DateTime, string, string, string) ParseString(string logLine)
        {
            //Нахождение раздела строк
            int indexTime1 = logLine.IndexOf(' ');
            int indexTime2 = logLine.IndexOf(' ', indexTime1 + 1);
            int indexOpenLevel = logLine.IndexOf('[');
            int indexCloseLevel = logLine.IndexOf(']');
            int indexOpenCategory = logLine.IndexOf('[', indexOpenLevel + 1);
            int indexCloseCategory = logLine.IndexOf(']', indexCloseLevel + 1);
            int indexMessage = logLine.IndexOf(' ', indexTime2 + 1);
            //Подстроки
            string timeString = logLine.Substring(0, indexTime2);
            string levelString = logLine.Substring(indexOpenLevel + 1, indexCloseLevel - indexOpenLevel - 1);
            string categoryString = logLine.Substring(indexOpenCategory + 1, indexCloseCategory - indexOpenCategory - 1);
            string message = logLine.Substring(indexMessage + 1, logLine.Length - indexMessage - 1);
            //Перевод в данные
            DateTime logTime = DateTime.Parse(timeString);
            //Возвращаем данные
            return (logTime, levelString, categoryString, message);
        }

        public static string BuildReport(string[] lines)
        {
            string reportString = "";

            //Объявление переменных
            //Имя события
            string eventName = null;
            //Дата начала события
            DateOnly startDate = DateOnly.MinValue;
            //Дата конца события
            DateOnly endDate = DateOnly.MaxValue;
            //Строка начала события
            int startIndex = -1;
            //Строка конца события
            int endIndex = -1;
            //Победитель
            string winnerName = null;
            //Количество очков победителей
            int winnerPoints = 0;
            //Предмет события
            string eventItems = "";
            //Лузеры и их утешительные призы
            string loserMessages = "";
            //Количество предупреждений
            int warningCount = 0;
            //Количество ошибок
            int errorCount = 0;

            //Нахождения события, его начала и конца
            for (int i = 0; i < lines.Length; i++)
            {
                //Нахождения начала события
                if (lines[i].Contains("Событие началось:"))
                {
                    //Парсим строку на составные части
                    (
                        DateTime time,
                        string level,
                        string category,
                        string message
                    ) = ParseString(lines[i]);
                    //И достаем нужные данные
                    startDate = new DateOnly(time.Year, time.Month, time.Day);
                    startIndex = i;
                    int indexStartName = message.IndexOf(":");
                    eventName = message.Substring(indexStartName + 2, message.Length - indexStartName - 2);
                }
                //Нахождения конца события
                if (lines[i].Contains($"Событие \"{eventName}\" закрыто"))
                {
                    //Парсим строку на составные части
                    (
                        DateTime time,
                        string level,
                        string category,
                        string message
                    ) = ParseString(lines[i]);
                    //И достаем нужные данные
                    endDate = new DateOnly(time.Year, time.Month, time.Day);
                    endIndex = i;
                }
            }

            //Возращает ошибочную строку, если нет события
            if (startIndex == -1 || endIndex == -1)
            {
                return "Событие не было найдено";
            }

            //Если дата начала и конца события разная, пишем в формате <Дата начала события>\<Дата конца события>
            string eventDate = startDate.ToString();
            if (startDate != endDate)
            {
                eventDate = startDate.ToString() + "\\" + endDate.ToString(); ;
            }



            //Просмотр строк ивента
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (lines[i].Contains("Warning"))
                {
                    warningCount++;
                }
                if (lines[i].Contains("Error"))
                {
                    errorCount++;
                }
                if (lines[i].Contains($"объявлены победителями события \"{eventName}\""))
                {
                    //Парсим строку на составные части
                    (
                        DateTime time,
                        string level,
                        string category,
                        string message
                    ) = ParseString(lines[i]);
                    int index = message.IndexOf($"объявлены победителями события \"{eventName}\"");
                    winnerName = message.Substring(0, index-1);
                    //Т.К мы нашли победителя, то смотрим сколько они заработали баллов и ивентовые предметы
                    for(int y = i; y>=0 ; y--)
                    {
                        if (lines[y].Contains($"{winnerName} получили ") && lines[y].Contains(" очков события"))
                        {
                            //Парсим строку на составные части
                            (
                                time,
                                level,
                                category,
                                message
                            ) = ParseString(lines[y]);
                            string[] splitedMessage = message.Split(' ');
                            winnerPoints += Int32.Parse(splitedMessage[winnerName.Split(' ').Length + 1]);
                        }
                        if (lines[y].Contains($"{winnerName} получили ивентовый предмет: "))
                        {
                            {
                                //Парсим строку на составные части
                                (
                                    time,
                                    level,
                                    category,
                                    message
                                ) = ParseString(lines[y]);
                                int itemStartIndex = message.IndexOf(':');
                                eventItems += message.Substring(itemStartIndex+2, message.Length - itemStartIndex-2);
                            }
                        }
                    }
                }
                if (lines[i].Contains("утешительную награду:"))
                {
                    //Парсим строку на составные части
                    (
                        DateTime time,
                        string level,
                        string category,
                        string message
                    ) = ParseString(lines[i]);
                    string[] splitedMessage = message.Split(' ');
                    int endNameIndex = message.IndexOf(" получили утешительную награду:");
                    string loserName = message.Substring(0, endNameIndex);
                    int loserPoints = Int32.Parse(splitedMessage[loserName.Split(' ').Length+3]);
                    loserMessages = $"Утешительная награда {loserName}: {loserPoints} очков\n";
                }

            }

            reportString =
            $"# Итоги события: {eventName}\n" +
            $"\n" +
            $"Дата: {eventDate}\n" +
            $"Победитель: {winnerName}\n" +
            $"Очки победителя: {winnerPoints}\n" +
            $"Ивентовый предмет: {eventItems}\n" +
            loserMessages +
            $"Предупреждений во время события: {warningCount}\n" +
            $"Ошибок во время события: {errorCount}\n";

            return reportString;

        }

        /// <summary>
        /// Функция для поиска строк лога
        /// </summary>
        /// <param name="logLines"></param>
        /// <param name="searchWord"></param>
        static void WriteAllNeedStrings(string[] logLines, string searchWord)
        {
            for (int i = 0; i < logLines.Length; i++)
            {
                if (logLines[i].Contains(searchWord))
                {
                    Console.WriteLine($"{i + 1}\\{logLines[i]}");
                }
            }
        }

        static void Main(string[] args)
        {
            const string FilePath = "./logs/event_server.log";
            if (File.Exists(FilePath))
            {
                string[] logLines = File.ReadAllLines(FilePath);
                Console.WriteLine(BuildReport(logLines));
            }
            else
            {
                Console.WriteLine("Файл с отчетом не было найдено");
            }
        }
    }
}
