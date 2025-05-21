using KabreetGames.BladeSpinner.TowerSystem.Stats;
using KabreetGames.EventBus.Interfaces;

namespace KabreetGames.BladeSpinner.Events
{
    public struct OnWaveFillUpdate : IEvent
    {
        public readonly float fill;

        [UnityEngine.Scripting.Preserve]
        public OnWaveFillUpdate(float fill)
        {
            this.fill = fill;
        }
    }

    public struct OnWaveUnlocked : IEvent
    {
        public readonly int waveNumber;

        [UnityEngine.Scripting.Preserve]
        public OnWaveUnlocked(int waveNumber)
        {
            this.waveNumber = waveNumber;
        }
    }  
    
    public struct OnGameLost : IEvent
    {
    }   
    public struct OnGameWin : IEvent
    {
    }

    public struct OnUpdateClicked : IEvent
    {
        public readonly Stat stat;
        public readonly StatModifier statModifier;

        [UnityEngine.Scripting.Preserve]
        public OnUpdateClicked(Stat stat, StatModifier statModifier)
        {
            this.stat = stat;
            this.statModifier = statModifier;
        }
    }

    public struct OnCoinsChanged : IEvent
    {
        public readonly int coins;
        
        [UnityEngine.Scripting.Preserve]
        public OnCoinsChanged(int coins)
        {
            this.coins = coins;
        }
    }
    public struct OnTowerDie : IEvent
    {
        public readonly int towerXp;

        [UnityEngine.Scripting.Preserve]
        public OnTowerDie(int towerXp)
        {
            this.towerXp = towerXp;
        }
    }
    
    public struct OnSpinnerAreaChange : IEvent
    {
        public readonly ISpinner spinner;    
        public readonly Area area;

        [UnityEngine.Scripting.Preserve]
        public OnSpinnerAreaChange(ISpinner spinner, Area area)
        {
            this.spinner = spinner;
            this.area = area;
        }
    }
}