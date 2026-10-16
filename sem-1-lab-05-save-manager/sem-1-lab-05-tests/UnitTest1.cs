
using sem_1_lab_05_save_manager;
using sem_1_lab_05_save_manager.Models;

namespace sem_1_lab_05_tests
{
    [TestFixture]
    public class SaveManagerTests
    {
        private string _testDirectory;
        private string _indexPath;
        private string _savesDirectory;
        private string _backupsDirectory;

        [SetUp]
        public void SetUp()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "SaveManagerTests-" + Guid.NewGuid());
            _indexPath = Path.Combine(_testDirectory, "index.json");
            _savesDirectory = Path.Combine(_testDirectory, "saves");
            _backupsDirectory = Path.Combine(_testDirectory, "backups");

            Directory.CreateDirectory(_savesDirectory);
            Directory.CreateDirectory(_backupsDirectory);

            File.WriteAllText(_indexPath,
                "[{\"name\":\"slot-01\",\"file\":\"save-01.json\",\"updatedAt\":\"2026-04-18T10:15:00\"}]");
            File.WriteAllText(Path.Combine(_savesDirectory, "save-01.json"),
                "{\"character\":\"Mira\",\"map\":\"Ashen Coast\",\"position\":{\"x\":128,\"y\":47},\"health\":76,\"inventory\":[\"iron sword\"]}");
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Test]
        public void LoadIndex_ReadsSaveEntry()
        {
            List<SaveEntry> entries = Program.LoadIndex(_indexPath);

            Assert.That(entries, Has.Count.EqualTo(1));
            Assert.That(entries[0].name, Is.EqualTo("slot-01"));
            Assert.That(entries[0].file, Is.EqualTo("save-01.json"));
        }

        [Test]
        public void FindByName_IsCaseInsensitive()
        {
            List<SaveEntry> entries = Program.LoadIndex(_indexPath);

            SaveEntry entry = Program.FindByName(entries, "SLOT-01");

            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.name, Is.EqualTo("slot-01"));
        }

        [Test]
        public void FindByName_ReturnsNullWhenSaveDoesNotExist()
        {
            List<SaveEntry> entries = Program.LoadIndex(_indexPath);

            Assert.That(Program.FindByName(entries, "missing"), Is.Null);
        }

        [Test]
        public void LoadSave_ReadsCharacterAndWorldData()
        {
            SaveEntry entry = Program.LoadIndex(_indexPath)[0];

            SaveData save = Program.LoadSave(_savesDirectory, entry);

            Assert.That(save.Character, Is.EqualTo("Mira"));
            Assert.That(save.Map, Is.EqualTo("Ashen Coast"));
            Assert.That(save.Position.x, Is.EqualTo(128));
            Assert.That(save.Position.y, Is.EqualTo(47));
            Assert.That(save.Health, Is.EqualTo(76));
            Assert.That(save.Inventory, Does.Contain("iron sword"));
        }

        [Test]
        public void SaveGame_WritesEditedDataThatCanBeLoadedAgain()
        {
            SaveEntry entry = Program.LoadIndex(_indexPath)[0];
            SaveData save = Program.LoadSave(_savesDirectory, entry);
            save.Map = "Crystal Cavern";
            save.Position.x = 10;
            save.Position.y = 25;
            save.Health = 50;
            save.Inventory.Add("crystal key");

            Program.SaveGame(_savesDirectory, entry, save);
            SaveData loaded = Program.LoadSave(_savesDirectory, entry);

            Assert.That(loaded.Map, Is.EqualTo("Crystal Cavern"));
            Assert.That(loaded.Position.x, Is.EqualTo(10));
            Assert.That(loaded.Position.y, Is.EqualTo(25));
            Assert.That(loaded.Health, Is.EqualTo(50));
            Assert.That(loaded.Inventory, Does.Contain("crystal key"));
        }

        [Test]
        public void SaveIndex_WritesEntriesThatCanBeLoadedAgain()
        {
            List<SaveEntry> entries = Program.LoadIndex(_indexPath);
            entries.Add(new SaveEntry
            {
                name = "manual",
                file = "manual.json",
                updatedAt = new DateTime(2026, 5, 1, 12, 30, 0)
            });

            Program.SaveIndex(_indexPath, entries);
            List<SaveEntry> loaded = Program.LoadIndex(_indexPath);

            Assert.That(loaded, Has.Count.EqualTo(2));
            Assert.That(loaded[1].name, Is.EqualTo("manual"));
            Assert.That(loaded[1].updatedAt, Is.EqualTo(new DateTime(2026, 5, 1, 12, 30, 0)));
        }

        [Test]
        public void CreateBackup_CopiesSaveFile()
        {
            SaveEntry entry = Program.LoadIndex(_indexPath)[0];

            Program.CreateBackup(_savesDirectory, _backupsDirectory, entry);

            string backupPath = Path.Combine(_backupsDirectory, entry.file);
            Assert.That(File.Exists(backupPath), Is.True);
            Assert.That(File.ReadAllText(backupPath), Is.EqualTo(File.ReadAllText(Path.Combine(_savesDirectory, entry.file))));
        }

        [Test]
        public void RestoreBackup_ReplacesCurrentSaveWithBackup()
        {
            SaveEntry entry = Program.LoadIndex(_indexPath)[0];
            Program.CreateBackup(_savesDirectory, _backupsDirectory, entry);
            File.WriteAllText(Path.Combine(_savesDirectory, entry.file), "changed");

            Program.RestoreBackup(_savesDirectory, _backupsDirectory, entry);

            Assert.That(File.ReadAllText(Path.Combine(_savesDirectory, entry.file)),
                Does.Contain("Ashen Coast"));
        }

        [Test]
        public void DeleteSave_RemovesFileAndIndexEntry()
        {
            List<SaveEntry> entries = Program.LoadIndex(_indexPath);
            SaveEntry entry = entries[0];

            Program.DeleteSave(_savesDirectory, _indexPath, entries, entry);

            Assert.That(File.Exists(Path.Combine(_savesDirectory, entry.file)), Is.False);
            Assert.That(Program.LoadIndex(_indexPath), Is.Empty);
        }
    }

}
