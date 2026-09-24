namespace ServerConfigProgram
{
    public class Program
    {
        //
        const int playerOnRamGb = 10;

        public static string CheckConfiguration(int playerCount, int ramInGb, bool networkConnection, bool isPassworded)
        {
            //
            int MaxRecommendPlayerCount = ramInGb*playerOnRamGb;
            //
            if (!networkConnection) {
                return "Запуск невозможен: сервер должен быть соединён с сетью";
            }
            if (playerCount <= 0)
            {
                return "Запуск невозможен: количество игроков должно быть больше нуля.";
            }
            //
            if (ramInGb < 4)
            {
                return "Запуск невозможен: серверу недостаточно оперативной памяти.";
            }
            //
            if (MaxRecommendPlayerCount < playerCount) {
                return "Запуск возможен с предупреждением: для такого количества игроков рекомендуется больше оперативной памяти.";
            }
            //
            if(isPassworded)
            {
                return "Запуск возможен с предупреждением: публичный сервер защищён паролем.";
            }
            //
            return "Сервер готов к запуску.";
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
