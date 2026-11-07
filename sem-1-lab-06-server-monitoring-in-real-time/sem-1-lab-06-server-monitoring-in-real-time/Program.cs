namespace lab_1_sem_06_server_monitoring_in_real_time
{
    public class Program
    {
        // Создаём массивы, для сохранения последних 20 значений 
        private static double[] cpuArray = new double[20];
        private static double[] ramArray = new double[20];
        private static double[] temperatureArray = new double[20];
        public static void RenderGraphic(double[] array)
        {
            for (int i = 10; i >= 1; i--) {
                if (i * 10 >= 100)
                {
                    Console.Write($"{i * 10} |");
                }
                else
                {
                    Console.Write($"{i * 10}  |");
                }
                for (int j = 0; j <= 19; j++) {
                    if (i*10-5 <= array[j])
                    {
                        Console.Write("#");
                    }
                    else 
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("----+--------------------");
        }
        private static double[] addNewValue(double[] array, double newValue)
        {
            for (int i = array.Length-1; i >= 1; i--)
            {
                array[i] = array[i - 1];
            }
            array[0] = newValue;
            return array;
        }

        static void Main(string[] args)
        {
            // Создаём объект, который будет получать данные о состоянии сервера.
            ServerMonitor monitor = new ServerMonitor();



            // Сообщаем библиотеке, какой метод нужно вызывать
            // каждый раз, когда появляются новые данные.
            monitor.DataReceived += OnDataReceived;

            // Запускаем получение данных.
            // После этого библиотека сама будет передавать новые значения примерно раз в секунду.
            monitor.Start();

            Console.WriteLine("Мониторинг запущен.");
            Console.WriteLine("Для завершения программы нажмите Enter.");

            // Не даём программе завершиться сразу после запуска.
            Console.ReadLine();

            // При желании можно остановить получение данных.
            monitor.Stop();
        }

        private static void OnDataReceived(double cpu, double ram, double temperature)
        {
            // Этот метод автоматически вызывается библиотекой
            // каждый раз, когда появляются новые данные.

            // cpu — текущая загрузка процессора (в процентах).
            // ram — использование оперативной памяти (в процентах).
            // temperature — температура процессора (в градусах Цельсия).
            for (int i = 19; i >= 0; i--)
            {
                if (cpuArray[i] != 0)
                {
                    Console.WriteLine($"CPU: {cpuArray[i]}%/RAM: {ramArray[i]}%/Temperature: {temperatureArray[i]} °C");
                }
            }
            // Вместо этих строк вы можете написать свой код.
            // Например:
            // - сохранить значения в массивы;
            cpuArray = addNewValue(cpuArray, cpu);
            ramArray = addNewValue(ramArray, ram);
            temperatureArray = addNewValue(temperatureArray, temperature);
            // - обновить график;
            Console.WriteLine("CPU");
            RenderGraphic(cpuArray);
            Console.WriteLine("RAM");
            RenderGraphic(ramArray);
            // - вычислить статистику;
            // - вывести информацию в другом формате.
        }
    }
}
