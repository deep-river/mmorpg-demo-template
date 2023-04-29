using Common;
using Common.Data;
using GameServer.Models;

namespace GameServer.Managers
{
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;

        // 刷新时间
        private float spawnTime = 0;

        // 消失时间
        private float unspawnTime = 0;

        private bool spawned = false;

        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine define, Map map)
        {
            Define = define;
            Map = map;

            if (DataManager.Instance.SpawnPoints.ContainsKey(Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[Map.ID].ContainsKey(Define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[map.ID][Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("SpawnRule[{0}] SpawnPoint[{1}] does not exist", this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        public void Update()
        {
            if (this.CanSpawn())
            {
                this.Spawn();
            }
        }

        bool CanSpawn()
        {
            if (this.spawned)
                return false;
            if (this.unspawnTime + this.Define.SpawnPeriod > Time.time)
                return false;

            return true;
        }

        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}] Spawn[{1}:Monster:{2},Lv:{3} at Point:{4}]", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}
