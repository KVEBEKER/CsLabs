using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L1S01GameServerMonitoring.Model
{
    internal class ServerState
    {
        //Системные параметры:
        //Операционная система
        public string Oc { get; }
        //Доступные процессоры
        public int AvailableProcessorCount { get; }
        //Общая оперативная память
        public float TotalRam { get; }
        //Свободная оперативная память
        public float FreeRam { get; }
        //Общая системная память
        public float TotalMemory { get; }

        //Игровые параметры:
        //Максимальное количество игроков
        public int MaxPlayerCount { get; }
        //Текущие количество игроков
        public int PlayerCount { get; }
        //Сохранённые данные об игроках
        public List<Player> SavedPlayers {  get; }
        public ServerState() { }

        public ServerState(
            string oc, 
            int availableProcessorCount, 
            float totalRam, 
            float freeRam, 
            float totalMemory, 
            int maxPlayerCount, 
            int playerCount, 
            List<Player> savedPlayers)
        {
            this.Oc = oc;
            this.AvailableProcessorCount = availableProcessorCount;
            this.TotalRam = totalRam;
            this.FreeRam = freeRam;
            this.TotalMemory = totalMemory;
            this.MaxPlayerCount = maxPlayerCount;
            this.PlayerCount = playerCount;
            this.SavedPlayers = savedPlayers;
        }
    }
}
