using System.Collections.Generic;
using KabreetGames.BladeSpinner.TowerSystem.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KabreetGames.BladeSpinner
{
    public class WaveUpgradeData : ScriptableSingleton<WaveUpgradeData>
    {
        [field: SerializeField, SerializeReference]
        public List<WaveUpgrade> waveUpgrades;

        [field: SerializeField, SerializeReference]
        public List<Stat> Stats { get; private set; } = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Instance.ResetStats();
        }


        public void ResetStats()
        {
            foreach (var stat in Stats)
            {
                stat.OnRest();
            }
        }


        public List<string> GetStatNames()
        {
            return Stats.ConvertAll(stat => stat.statName);
        }


        public void AddWaveUpgrade(WaveUpgrade waveUpgrade)
        {
            waveUpgrades.Add(waveUpgrade);
        }

        public void RemoveWaveUpgrade(WaveUpgrade waveUpgrade)
        {
            waveUpgrades.Remove(waveUpgrade);
        }

        private WaveUpgrade GetRandomWaveUpgrade()
        {
            return waveUpgrades[Random.Range(0, waveUpgrades.Count)];
        }

        public WaveUpgrade GetUniqueRandomWaveUpgrade(IList<WaveUpgrade> existingWaveUpgrades = null)
        {
            var uniqueWaveUpgrade = GetRandomWaveUpgrade();
            while (existingWaveUpgrades != null && existingWaveUpgrades.Contains(uniqueWaveUpgrade) &&
                   existingWaveUpgrades.Count < waveUpgrades.Count)
            {
                uniqueWaveUpgrade = GetRandomWaveUpgrade();
            }

            return uniqueWaveUpgrade;
        }

        public List<WaveUpgrade> GetUniqueRandomWaveUpgrade(int count)
        {
            var temp = new List<WaveUpgrade>();
            for (var i = 0; i < count; i++)
            {
                temp.Add(GetUniqueRandomWaveUpgrade(temp));
            }

            return temp;
        }


        public T GetStatValue<T>(string statName) where T : struct
        {
            return TryGetStat(statName, out var stat) ? ((Stat<T>)stat).Value : default;
        }

        public Stat<T> GetStat<T>(string statName) where T : struct
        {
            if (TryGetStat(statName, out var stat))
            {
                return stat as Stat<T>;
            }

            //Create a new stat
            var newStat = new Stat<T>(default, statName);
            AddStat(newStat);
            return newStat;
        }

        private void AddStat(Stat stat)
        {
            Stats.Add(stat);
        }

        public void RemoveStat(string statName)
        {
            Stats.RemoveAll(stat => stat.statName == statName);
        }

        public void AddFloatStat(string statName, float startValue)
        {
            if (Stats.Exists(stat => stat.statName == statName))
                return;
            var stat = new Stat<float>(startValue, statName);
            AddStat(stat);
        }

        public void AddIntStat(string statName, int startValue)
        {
            if (Stats.Exists(stat => stat.statName == statName))
                return;
            var stat = new Stat<int>(startValue, statName);
            AddStat(stat);
        }

        public bool TryGetStat(string statName, out Stat stat)
        {
            stat = Stats.Find(s => s.statName == statName);
            return stat != null;
        }
    }
}