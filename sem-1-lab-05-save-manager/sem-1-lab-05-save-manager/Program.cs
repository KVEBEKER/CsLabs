using sem_1_lab_05_save_manager.Models;
using Newtonsoft.Json;
using sem_1_lab_05_save_manager.models;

namespace sem_1_lab_05_save_manager
{
    public class Program
    {
        const string IndexDirectory = "./index.json";
        const string SaveDirectory = "./saves";
        const string BackupDirectory = "./backups";
        const string LogDirectory = "./logs.json";
        public static void addLogLine(string logDirectory, string fileName, string operation)
        {
            if (!File.Exists(logDirectory))
            {
                string empty = "";
                File.WriteAllText(logDirectory, empty);
            }
            string json = File.ReadAllText(logDirectory);
            List<SaveLog> logs = JsonConvert.DeserializeObject<List<SaveLog>>(json);
            SaveLog newLog = new SaveLog();
            newLog.makeAt = DateTime.Now;
            newLog.name = fileName;
            newLog.operation = operation;
            logs.Add(newLog);
            string newJson = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(logDirectory, newJson);
        }

        public static List<SaveEntry> LoadIndex(string indexPath)
        {
            if (!File.Exists(indexPath))
            {
                return new List<SaveEntry>();
            }
            string json = File.ReadAllText(indexPath);
            List<SaveEntry> result = JsonConvert.DeserializeObject<List<SaveEntry>>(json);
            return result;
        }
        public static SaveData LoadSave(string savesDirectory, SaveEntry entry)
        {

            string saveDirectory = savesDirectory + "/" + entry.file;
            if (!File.Exists(saveDirectory))
            {
                Console.WriteLine($"Файл не существует в: {saveDirectory}");
            }
            string json = File.ReadAllText(saveDirectory);
            SaveData save = JsonConvert.DeserializeObject<SaveData>(json);
            return save;
        }
        public static void PrintSave(SaveEntry entry, SaveData save)
        {
            Console.WriteLine($"Имя сохранения: {entry.name}");
            Console.WriteLine($"Имя персонажа: {save.Character}");
            Console.WriteLine($"Имя карты: {save.Map}");
            Console.WriteLine("Координаты");
            Console.WriteLine($"X: {save.Position.x}");
            Console.WriteLine($"Y: {save.Position.y}");
            Console.WriteLine($"Здоровья: {save.Health}");
            Console.WriteLine("Предметы:");
            foreach (string item in save.Inventory)
            {
                Console.WriteLine($"{item}");
            }
        }
        public static SaveEntry FindByName(List<SaveEntry> entries, string name)
        {
            SaveEntry? entry = entries.Find(x => x.name == name.ToLower());
            if (entry == null)
            {
                return new SaveEntry();
            }
            return entry;
        }
        public static void SaveGame(string savesDirectory, SaveEntry entry, SaveData save)
        {
            string json = JsonConvert.SerializeObject(save, Formatting.Indented);
            string saveDirectory = savesDirectory + "/" + entry.file;
            File.WriteAllText(saveDirectory, json);
        }
        public static void SaveIndex(string indexPath, List<SaveEntry> entries)
        {
            string json = JsonConvert.SerializeObject(entries, Formatting.Indented);
            File.WriteAllText(indexPath, json);
        }

        public static void CreateBackup(string savesDirectory, string backupsDirectory, SaveEntry entry)
        {
            string saveDirectory = savesDirectory + "/" + entry.file;
            string saveBackupDirectory = backupsDirectory + "/" + entry.file;
            File.Copy(saveDirectory, saveBackupDirectory, true);
            Console.WriteLine("Бэкап успешно создан");
        }
        public static void RestoreBackup(string savesDirectory, string backupsDirectory, SaveEntry entry)
        {
            string saveDirectory = savesDirectory + "/" + entry.file;
            string saveBackupDirectory = backupsDirectory + "/" + entry.file;
            if (!File.Exists(saveBackupDirectory))
            {
                Console.WriteLine("Бэкапа этого файла не существует.");
                return;
            }
            File.Copy(saveBackupDirectory, saveDirectory, true);
            Console.WriteLine("Бэкап успешно загружен");
        }
        public static void DeleteSave(string savesDirectory, string indexPath, List<SaveEntry> entries, SaveEntry entry)
        {
            string saveDirectory = savesDirectory + "/" + entry.file;
            if (File.Exists(saveDirectory))
            {
                File.Delete(saveDirectory);
                Console.WriteLine("Файл успешно удалён");
            }
            else
            {
                Console.WriteLine("Файла на месте сохранения не было. Удаляю индекс");
            }
            entries.Remove(entry);
            string newJson = JsonConvert.SerializeObject(entries, Formatting.Indented);
            File.WriteAllText(saveDirectory, newJson);
        }

        private static void PrintSaveList(List<SaveEntry> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                SaveEntry entry = entries[i];
                Console.WriteLine($"{i + 1}. {entry.name}|{entry.file}|{entry.updatedAt}");
            }
        }

        private static void PrintChosenSave(List<SaveEntry> entries)
        {
            PrintSaveList(entries);
            int option = -1;
            Console.WriteLine("Выберите сохранение, написав его номер");
            if (int.TryParse(Console.ReadLine(), out option))
            {
                try
                {
                    SaveEntry entry = entries[option - 1];
                    SaveData data = LoadSave(SaveDirectory, entry);
                    PrintSave(entry, data);
                }
                catch
                {
                    Console.WriteLine("Был выбран неверный символ, попробуйте ещё раз");
                    return;
                }
            }

        }

        private static void ChangeAndSave(List<SaveEntry> entries)
        {
            PrintSaveList(entries);
            int option = -1;
            Console.WriteLine("Выберите сохранение, написав его номер");
            if (int.TryParse(Console.ReadLine(), out option))
            {
                try
                {
                    SaveEntry entry = entries[option - 1];
                    SaveData data = LoadSave(SaveDirectory, entry);
                    bool cycle = true;
                    while (cycle)
                    {
                        Console.Clear();
                        PrintSave(entry, data);
                        int subOption = -1;
                        Console.WriteLine("Что хотите поменять?");
                        Console.WriteLine("1. Имя персонажа.");
                        Console.WriteLine("2. Место");
                        Console.WriteLine("3. Здоровье персонажа");
                        Console.WriteLine("4. Сохранить и выйти");
                        Console.WriteLine("0. Выйти.");
                        if (int.TryParse(Console.ReadLine(), out subOption))
                        {
                            switch (option)
                            {
                                case 1:
                                    data.Character = ChangeCharacterName(data.Character);
                                    break;
                                case 2:
                                    (data.Map, data.Position.x, data.Position.y) = ChangeCharacterPlace(data.Map, data.Position.x, data.Position.y);
                                    break;
                                case 3:
                                    data.Health = ChangeCharacterHealth(data.Health);
                                    break;
                                case 4:
                                    cycle = false;
                                    SaveGame(SaveDirectory, entry, data);
                                    Console.WriteLine("Изменения сохранены");
                                    break;
                                case 0:
                                    cycle = false;
                                    break;
                            }
                        }
                    }


                }
                catch
                {
                    Console.WriteLine("Был выбран неверный символ, попробуйте ещё раз");
                    return;
                }
            }
        }

        private static int ChangeCharacterHealth(int oldHealth)
        {
            Console.WriteLine("Введите целое положительное число.");
            int newHealth = oldHealth;
            string buffer = Console.ReadLine();
            if (int.TryParse(buffer, out newHealth))
            {
                if (newHealth < 0)
                {
                    Console.WriteLine("Здоровье не может быть отрицательным");
                    return oldHealth;
                }
            }
            else
            {
                Console.WriteLine("Не введено число.");
                return oldHealth;
            }
            return newHealth;
        }

        private static string ChangeCharacterName(string oldName)
        {
            Console.WriteLine("Введите новое имя для персонажа. Если хотите оставить старое имя - ничего не пишите");
            string newName = Console.ReadLine();
            if (newName == string.Empty)
            {
                return oldName;
            }
            return newName;
        }

        private static (string, int, int) ChangeCharacterPlace(string oldMap, int oldX, int oldY)
        {
            string newMap = oldMap;
            int newX = oldX;
            int newY = oldY;
            Console.WriteLine("Каким образом меняется место?");
            Console.WriteLine("1. Новая карта");
            Console.WriteLine("2. Только новые координаты");
            Console.WriteLine("3. Ничего не менять");
            string buffer = string.Empty;
            int option = -1;
            if (int.TryParse(Console.ReadLine(), out option))
            {
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Какая называется карта?");
                        buffer = Console.ReadLine();
                        newMap = buffer;
                        Console.WriteLine("Введите целое число для координата X");
                        buffer = Console.ReadLine();
                        if (int.TryParse(buffer, out newX))
                        {
                            Console.WriteLine("Число принято");
                        }
                        else
                        {
                            Console.WriteLine("Не введено число.");
                        }
                        Console.WriteLine("Введите целое число для координата X");
                        buffer = Console.ReadLine();
                        if (int.TryParse(buffer, out newY))
                        {
                            Console.WriteLine("Число принято");
                        }
                        else
                        {
                            Console.WriteLine("Не введено число.");
                        }
                        break;
                    case 2:
                        Console.WriteLine("Введите целое число для координата X");
                        buffer = Console.ReadLine();
                        if (int.TryParse(buffer, out newX))
                        {
                            Console.WriteLine("Число принято");
                        }
                        else
                        {
                            Console.WriteLine("Не введено число.");
                        }
                        Console.WriteLine("Введите целое число для координата X");
                        buffer = Console.ReadLine();
                        if (int.TryParse(buffer, out newY))
                        {
                            Console.WriteLine("Число принято");
                        }
                        else
                        {
                            Console.WriteLine("Не введено число.");
                        }
                        break;
                    case 3:
                        break;
                }
            }
            else
            {
                Console.WriteLine("Не введено число.");
            }
            return (newMap, newX, newY);
        }

        private static void CreateSaveBackup(List<SaveEntry> entries)
        {
            PrintSaveList(entries);
            int option = -1;
            Console.WriteLine("Выберите сохранение, написав его номер");
            if (int.TryParse(Console.ReadLine(), out option))
            {
                try
                {
                    SaveEntry entry = entries[option - 1];
                    CreateBackup(SaveDirectory, BackupDirectory, entry);
                }
                catch
                {
                    Console.WriteLine("Был выбран неверный символ, попробуйте ещё раз");
                    return;
                }
            }
        }

        private static void UploadSaveBackup(List<SaveEntry> entries)
        {
            PrintSaveList(entries);
            int option = -1;
            Console.WriteLine("Выберите сохранение, написав его номер");
            if (int.TryParse(Console.ReadLine(), out option))
            {
                try
                {
                    SaveEntry entry = entries[option - 1];
                    RestoreBackup(SaveDirectory, BackupDirectory, entry);
                }
                catch
                {
                    Console.WriteLine("Был выбран неверный символ, попробуйте ещё раз");
                    return;
                }
            }
        }

        private static void DeleteChosenSave(List<SaveEntry> entries)
        {
            PrintSaveList(entries);
            int option = -1;
            Console.WriteLine("Выберите сохранение, написав его номер");
            if (int.TryParse(Console.ReadLine(), out option))
            {
                try
                {
                    SaveEntry entry = entries[option - 1];
                    SaveData data = LoadSave(SaveDirectory, entry);
                    DeleteSave(SaveDirectory, IndexDirectory, entries, entry);
                }
                catch
                {
                    Console.WriteLine("Был выбран неверный символ, попробуйте ещё раз");
                    return;
                }
            }
        }
        static void Main(string[] args)
        {
            List<SaveEntry> entries = LoadIndex(IndexDirectory);
            if (entries == null)
            {
                Console.WriteLine();
                return;
            }
            bool cycle = true;
            while (cycle)
            {
                int option = -1;
                Console.Clear();
                Console.WriteLine("Введите номер варианта:");
                Console.WriteLine("1. Показать список сохранений.");
                Console.WriteLine("2. Посмотреть данные сохранения.");
                Console.WriteLine("3. Изменить данные персонажа и записать их обратно.");
                Console.WriteLine("4. Создать резервную копию.");
                Console.WriteLine("5. Восстановить резервную копию.");
                Console.WriteLine("6. Удалить сохранение.");
                Console.WriteLine("7. Выйти.");
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option)
                    {
                        case 1:
                            Console.Clear();
                            PrintSaveList(entries);
                            Console.ReadKey();
                            break;
                        case 2:
                            Console.Clear();
                            PrintChosenSave(entries);
                            Console.ReadKey();
                            break;
                        case 3:
                            Console.Clear();
                            ChangeAndSave(entries);
                            Console.ReadKey();
                            break;
                        case 4:
                            Console.Clear();
                            CreateSaveBackup(entries);
                            Console.ReadKey();
                            break;
                        case 5:
                            Console.Clear();
                            UploadSaveBackup(entries);
                            Console.ReadKey();
                            break;
                        case 6:
                            Console.Clear();
                            DeleteChosenSave(entries);
                            Console.ReadKey();
                            break;
                        case 7:
                            cycle = false;
                            break;
                    }
                }
            }
        }
    }
}
