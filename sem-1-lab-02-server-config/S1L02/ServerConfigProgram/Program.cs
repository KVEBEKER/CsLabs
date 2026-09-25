using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ServerConfigProgram
{
    public class Program
    {
        //Максимальное рекомендованное количество игроков на гигобайт оперативной памяти
        const int playerOnRamGb = 10;

        /// <summary>
        /// Проверяет соединение
        /// </summary>
        /// <returns>выводит true, если соединение есть иначе false</returns>
        static bool CheckConnection()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет конфигурацию и выводит сообщение
        /// </summary>
        /// <param name="playerCount">Количество игроков</param>
        /// <param name="ramInGb">Оперативная память вашего сервера в гигабайтах</param>
        /// <param name="networkConnection">Есть ли подключения</param>
        /// <param name="isPassworded">Есть ли пароль</param>
        /// <returns></returns>
        public static string CheckConfiguration(int playerCount, int ramInGb, bool networkConnection, bool isPassworded)
        {
            //Максимальное рекомендованное количество игроков на производительность сервера
            int MaxRecommendPlayerCount = ramInGb * playerOnRamGb;
            //Ошибка: Нет подключения к Интернету
            if (!networkConnection)
            {
                return "Запуск невозможен: сервер должен быть соединён с сетью";
            }
            //Ошибка: Нет игроков
            if (playerCount <= 0)
            {
                return "Запуск невозможен: количество игроков должно быть больше нуля.";
            }
            //Ошибка: Не хватает оперативной памяти
            if (ramInGb < 4)
            {
                return "Запуск невозможен: серверу недостаточно оперативной памяти.";
            }
            //Предупреждения: Слишком много игроков для производительности сервера
            if (MaxRecommendPlayerCount < playerCount)
            {
                return "Запуск возможен с предупреждением: для такого количества игроков рекомендуется больше оперативной памяти.";
            }
            //Предупреждения: На сервер поставили пароль
            if (isPassworded)
            {
                return "Запуск возможен с предупреждением: публичный сервер защищён паролем.";
            }
            //Нормальное состояние
            return "Сервер готов к запуску.";
        }
        static void Main(string[] args)
        {
            //Ввод количество игроков
            Console.WriteLine("Сколько игроков на сервере?");
            int playersCount = 0;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out playersCount))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Ошибка ввода");
                    continue;
                }
            }
            //Ввод пароля
            bool isPassworded = false;
            string password = "";
            Console.WriteLine("Хотите добавить на сервер пароль? Напишите 1, если хотите добавить пароль");
            string buffer = Console.ReadLine();
            if (buffer == "1")
            {
                Console.WriteLine("Введите пароль, как минимум из 4 знаков");
                buffer = Console.ReadLine();
                if (buffer.Length >= 4)
                {
                    isPassworded = true;
                    password = buffer;
                }
            }
            //Вычисление количество гигабайтов данных
            ulong ramTotal = 0;
            using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    ramTotal = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                    break;
                }
            }
            int totalMemoryGb = (int)(ramTotal / 1024 / 1024 / 1024);
            Console.WriteLine(totalMemoryGb);
            //Проверить соединение
            bool isConnected = CheckConnection();
            Console.WriteLine(isConnected);
            Console.WriteLine(CheckConfiguration(playersCount, totalMemoryGb, isConnected, isPassworded));
        }
    }
}
