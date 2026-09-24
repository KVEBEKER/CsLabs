using L0PlayerProfile.Models;

namespace L0PlayerProfile
{
    internal class Program
    {

        /// <summary>
        /// Выводи профиль игрока
        /// </summary>
        /// <param name="profile"></param>
        static void RevealProfile(Player profile)
        {
            Console.WriteLine("=-=-=-=-=-=-=-=-=");
            Console.WriteLine($"Имя: {profile.name}");
            Console.WriteLine($"Раса: {profile.race}");
            Console.WriteLine($"Класс: {profile.gameClass}");
            Console.WriteLine($"Уровень: {profile.level.GetLevel()}");
            Console.WriteLine($"Опыт: {profile.level.GetExperiencePoints()}|{profile.level.GetNeedExperiencePoints()}");
            Console.WriteLine("=-=-=-=-=-=-=-=-=");
        }

        /// <summary>
        /// Создаёт новый профиль
        /// </summary>
        static Player CreateNewProfile()
        {
            // Ввод имени
            Console.WriteLine("Введите имя персонажа");
            string name = "";
            name = Console.ReadLine();
            Console.Clear();
            // Ввод расы
            string race = "";
            while (true)
            {
                int option = 0;
                Console.WriteLine("Введите цифру расы, которую хотите выбрать");
                Console.WriteLine("1. Человек");
                Console.WriteLine("2. Дворф");
                Console.WriteLine("3. Эльф");
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option)
                    {
                        case 1:
                            race = "Человек";
                            break;
                        case 2:
                            race = "Дворф";
                            break;
                        case 3:
                            race = "Эльф";
                            break;
                        default:
                            race = "Человек";
                            break;
                    }
                    // После успешное выбора - выход из цикла
                    break;
                }
                else
                {
                    Console.WriteLine("Неверное значение");
                }
                Console.ReadKey();
                Console.Clear();
            }
            // Ввод расы
            string gameClass = "";
            while (true)
            {
                int option = 0;
                Console.WriteLine("Введите цифру класса, которую хотите выбрать");
                Console.WriteLine("1. Воин");
                Console.WriteLine("2. Маг");
                Console.WriteLine("3. Вор");
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option)
                    {
                        case 1:
                            gameClass = "Воин";
                            break;
                        case 2:
                            gameClass = "Маг";
                            break;
                        case 3:
                            gameClass = "Вор";
                            break;
                        default:
                            gameClass = "Воин";
                            break;
                    }
                    // После успешное выбора - выход из цикла
                    break;
                }
                else
                {
                    Console.WriteLine("Неверное значение");
                }
                Console.ReadKey();
                Console.Clear();
            }
            // Создание игрока
            Player newPlayer = new Player(name, race, gameClass, new Level());
            Console.WriteLine("Ваш персонаж успешно создан");
            Console.ReadKey();
            // Возврат игрока
            return newPlayer;
        }

        static void Main(string[] args)
        {
            // Начальное создание профиля
            Console.WriteLine("Создайте свой первый профиль");
            Player player = CreateNewProfile();
            // Цикличное меню
            bool cycle = true;
            while (cycle)
            {
                // Появление сообщения меню
                Console.Clear();
                Console.WriteLine("Менеджер профиля игрока");
                Console.WriteLine("");
                Console.WriteLine("1. Показать профиль");
                Console.WriteLine("2. Добавить опыт");
                Console.WriteLine("3. Удалить профиль и создать новый");
                Console.WriteLine("0. Выйти из программы");
                // Ввод выбора
                string option = Console.ReadLine();
                // Обработка ввода и вывод
                Console.Clear();
                switch (option)
                {
                    // Показывает профиль
                    case "1":
                        RevealProfile(player);
                        Console.WriteLine("Нажмите чтобы продолжить");
                        Console.ReadKey();
                        break;
                    // Добавление опыта
                    case "2":
                        // Ввод опыта
                        Console.WriteLine("Напишете количество опыта которое хотите добавить:");
                        string expBuffer = Console.ReadLine();
                        int expPoints = 0;
                        if (int.TryParse(expBuffer, out expPoints))
                        {
                            player.AddExp(expPoints);
                            Console.WriteLine("Опыт успешно добавлен");
                        }
                        else
                        {
                            Console.WriteLine("Неверное значение");
                        }
                        Console.ReadKey();
                        break;
                    case "3":
                        Console.WriteLine("Вы точно уверены? Напишите +, если хотите продолжить");
                        string buffer = Console.ReadLine();
                        if (buffer == "+")
                        {
                            player = CreateNewProfile();
                        }
                        break;
                    // Выход из программы
                    case "0":
                        cycle = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
