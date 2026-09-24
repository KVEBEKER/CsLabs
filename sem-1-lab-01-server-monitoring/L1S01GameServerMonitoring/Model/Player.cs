using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace L1S01GameServerMonitoring.Model
{
    internal class Player
    {
        // Идентификационный номер персонажа
        public int Id { get; set; }
        // Имя персонажа
        public string name = "";
        // Раса персонажа
        public string race = "human";
        // Класс персонажа
        public string gameClass = "warrior";
        // Уровень персонажа
        public Level level;

        /// <summary>
        /// Создаёт базовый экземпляр класса с пустыми строками в параметрах
        /// </summary>
        public Player()
        {
            Id = 0;
            name = string.Empty;
            race = string.Empty;
            gameClass = string.Empty;
            level = new Level();
        }
        /// <summary>
        /// Создаёт экземпляр класса
        /// </summary>
        public Player(string name, string race, string gameClass, Level level)
        {
            this.Id = Random.Shared.Next();
            this.name = name;
            this.race = race;
            this.gameClass = gameClass;
            this.level = level;
        }
        /// <summary>
        /// Добавляет опыт к персонажу
        /// </summary>
        /// <param name="expPoints">Количество начисляемого опыта</param>
        public void AddExp(int expPoints)
        {
            level.AddExperiencePoints(expPoints);
        }
    }
}
