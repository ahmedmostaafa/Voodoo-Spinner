using System;
using KabreetGames.BladeSpinner.TowerSystem.Stats;
using UnityEngine;

namespace KabreetGames.BladeSpinner
{
    [Serializable]
    public class WaveUpgrade
    {
        public virtual void ApplyUpgrade()
        {
        }

        public virtual string Name { get; set; }
        [field: SerializeField] public virtual Sprite Icon { get; set; }
    }

    [Serializable]
    public class WaveUpgradeEvent
    {
        public readonly string waveUpgradeEventName;

        public WaveUpgradeEvent(string waveUpgradeEventName)
        {
            this.waveUpgradeEventName = waveUpgradeEventName;
        }
    }

    [Serializable]
    public class StatWaveUpgrade : WaveUpgrade
    {
        public override string Name => statModifier?.GetName(statName);
        [SerializeField] public string statName;
        [SerializeField, SerializeReference] public StatModifier statModifier;

        public StatWaveUpgrade(string statName, StatModifier statModifier)
        {
            this.statName = statName;
            this.statModifier = statModifier;
        }

        public override void ApplyUpgrade()
        {
            if (WaveUpgradeData.Instance.TryGetStat(statName, out var stat))
            {
                stat.AddModifier(statModifier);
                return;
            }

            Debug.LogError($"Stat {statName} not found");
        }
    }

    [Serializable]
    public class OtherWaveUpgrade : WaveUpgrade
    {
        public override string Name => displayName;
        public string displayName;

        public OtherWaveUpgrade(string displayName)
        {
            this.displayName = displayName;
        }
    }

    [Serializable]
    public class EventWaveUpgrade : WaveUpgrade
    {
        public override string Name => displayName;
        public string displayName;

        [SerializeReference] public string waveUpgradeEventName;

        public EventWaveUpgrade(string displayName, string waveUpgradeEventName)
        {
            this.displayName = displayName;
            this.waveUpgradeEventName = waveUpgradeEventName;
        }

        public override void ApplyUpgrade()
        {
            Manager.Instance.OnWaveUpgradeEvent.Invoke(new WaveUpgradeEvent(waveUpgradeEventName));
        }
    }
}