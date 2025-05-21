using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using KabreetGames.BladeSpinner.Events;
using KabreetGames.BladeSpinner.TowerSystem.Stats;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KabreetGames.BladeSpinner
{
    [DefaultExecutionOrder(-1000), DisallowMultipleComponent]
    public class Manager : Singleton<Manager>
    {
        private int waveXp;

        private readonly List<IUpdateObserver> updateObservers = new();
        private readonly List<IUpdateObserver> pendingUpdateObservers = new();
        private int currentUpdateObserverIndex;


        private readonly List<IFixedUpdateObserver> fixedUpdateObservers = new();
        private readonly List<IFixedUpdateObserver> pendingFixedUpdateObservers = new();
        private int currentFixedUpdateObserverIndex;


        public Stat<float> Force { get; private set; }
        public Stat<int> TotalWaves { get; private set; }
        public Stat<int> Damage { get; private set; }
        public Stat<int> Health { get; private set; }
        public Stat<int> WaveNumber { get; private set; } = 1;

        [SerializeField] private Area[] areas;

        public int Coins
        {
            get => coins;
            set
            {
                if (value < 0) value = 0;
                EventBus<OnCoinsChanged>.Rise(value);
                coins = value;
            }
        }

        private int coins;

        public Action<WaveUpgradeEvent> OnWaveUpgradeEvent { get; set; } = delegate { };

        public void Register(IUpdateObserver updateObserver)
        {
            if (updateObservers.Contains(updateObserver) || pendingUpdateObservers.Contains(updateObserver))
            {
                Debug.LogWarning("Already registered");
                return;
            }

            pendingUpdateObservers.Add(updateObserver);
        }

        public void Register(IFixedUpdateObserver updatable)
        {
            if (fixedUpdateObservers.Contains(updatable) || pendingFixedUpdateObservers.Contains(updatable))
            {
                Debug.LogWarning("Already registered");
                return;
            }

            pendingFixedUpdateObservers.Add(updatable);
        }

        public void Unregister(IUpdateObserver updateObserver)
        {
            if (!updateObservers.Contains(updateObserver))
            {
                Debug.LogWarning("Not registered");
            }

            updateObservers.Remove(updateObserver);
            currentUpdateObserverIndex--;
        }

        public void Unregister(IFixedUpdateObserver updatable)
        {
            if (!fixedUpdateObservers.Contains(updatable))
            {
                Debug.LogWarning("Not registered");
            }

            fixedUpdateObservers.Remove(updatable);
            currentFixedUpdateObserverIndex--;
        }

        public void Update()
        {
            for (currentUpdateObserverIndex = updateObservers.Count - 1;
                 currentUpdateObserverIndex >= 0;
                 currentUpdateObserverIndex--)
            {
                updateObservers[currentUpdateObserverIndex].ObserverUpdate();
            }

            updateObservers.AddRange(pendingUpdateObservers);
            pendingUpdateObservers.Clear();
        }

        private void OnEnable()
        {
            Damage = WaveUpgradeData.Instance.GetStat<int>("Damage");
            Force = WaveUpgradeData.Instance.GetStat<float>("Speed");
            Health = WaveUpgradeData.Instance.GetStat<int>("Health");
            TotalWaves = WaveUpgradeData.Instance.GetStat<int>("TotalWaves");
            EventBus<OnTowerDie>.Register(AddWaveXp);
            Health.OnValueChanged += HealthChanged;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnDisable()
        {
            EventBus<OnTowerDie>.Deregister(AddWaveXp);
            Health.OnValueChanged -= HealthChanged;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        private void Start()
        {
            EventBus<OnWaveFillUpdate>.Rise(waveXp = 0);
            EventBus<OnWaveUnlocked>.Rise(WaveNumber.baseValue = 1);
        }

        public void FixedUpdate()
        {
            for (currentFixedUpdateObserverIndex = fixedUpdateObservers.Count - 1;
                 currentFixedUpdateObserverIndex >= 0;
                 currentFixedUpdateObserverIndex--)
            {
                fixedUpdateObservers[currentFixedUpdateObserverIndex].ObserverFixedUpdate();
            }

            fixedUpdateObservers.AddRange(pendingFixedUpdateObservers);
            pendingFixedUpdateObservers.Clear();
        }

        private void AddWaveXp(OnTowerDie eve)
        {
            waveXp += eve.towerXp;
            EventBus<OnWaveFillUpdate>.Rise((float)waveXp / (WaveNumber * 3));
            if (waveXp < WaveNumber * 3) return;
            waveXp = 0;
            WaveNumber++;
            if (WaveNumber > TotalWaves)
            {
                WaveNumber = TotalWaves;
                EventBus<OnGameWin>.Rise();
                RuntimeManager.PlayOneShot("event:/Sfx/Win");
                return;
            }

            EventBus<OnWaveFillUpdate>.Rise(0);
            EventBus<OnWaveUnlocked>.Rise(WaveNumber.Value);
        }

        private void HealthChanged(int oldValue, int newValue)
        {
            if (newValue > 0) return;
            EventBus<OnGameLost>.Rise();
            RuntimeManager.PlayOneShot("event:/Sfx/Lose");
        }

        public Area GetWorkingArea(Vector2 transformPosition)
        {
            return areas.FirstOrDefault(area => InsideAreaFrame(transformPosition, area));
        }

        public Area GetRandomArea()
        {
            var a = areas.Where(area => !area.locked).ToArray();
            return a[Random.Range(0, a.Length)];
        }

        public static bool InsideAreaFrame(Vector2 transformPosition, Area workingArea)
        {
            var localPos = workingArea.transform.InverseTransformPoint(transformPosition);
            localPos.x /= workingArea.Size.x;
            localPos.y /= workingArea.Size.y;
            localPos += Vector3.one * 0.5f;
            return localPos.x is >= 0 and <= 1 && localPos.y is >= 0 and <= 1;
        }
    }


    public interface IUpdateObserver
    {
        void ObserverUpdate();
    }

    public interface IFixedUpdateObserver
    {
        void ObserverFixedUpdate();
    }
}