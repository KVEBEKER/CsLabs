using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L1S01GameServerMonitoring.Model
{
    internal class Level
    {
        // Требуемый опыт для новый уровня равен произведению этой константы и собственно уровня
        private const int needExperienceForLevelUpPerLevel = 1000;
        // Собственно уровень
        private int levelNumber;
        // Количество опыта
        private int experiencePoints;

        /// <summary>
        /// Базовое создание уровня с первым уровнем и без опыта
        /// </summary>
        public Level()
        {
            levelNumber = 1;
            experiencePoints = 0;
        }

        /// <summary>
        /// Добавляет очки опыта, если очков больше чем необходимого опыта, то уровень повышается а очки снимаются
        /// </summary>
        public void AddExperiencePoints(int newExperiencePoints)
        {
            experiencePoints += newExperiencePoints;
            while (experiencePoints >= needExperienceForLevelUpPerLevel * levelNumber)
            {
                experiencePoints -= needExperienceForLevelUpPerLevel * levelNumber;
                levelNumber++;
            }
        }
        /// <summary>
        /// Вывод уровня
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            return levelNumber;
        }
        /// <summary>
        /// Вывод очков опыта
        /// </summary>
        /// <returns></returns>
        public int GetExperiencePoints()
        {
            return experiencePoints;
        }
        /// <summary>
        /// Вывод необходимого количества очков опыта
        /// </summary>
        /// <returns></returns>
        public int GetNeedExperiencePoints()
        {
            return levelNumber * needExperienceForLevelUpPerLevel;
        }
    }
}
