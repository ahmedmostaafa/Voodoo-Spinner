using System;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using KabreetGames.BladeSpinner.Events;
using KabreetGames.BladeSpinner.TowerSystem.Stats;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using KabreetGames.TimeSystem;
using KabreetGames.UiSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.BladeSpinner.Ui
{
    public class MainPlay : MenuPanel
    {
        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Image waveFill;

        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private TMP_Text waveNumber;


        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private TMP_Text healthNumber;

        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private TMP_Text coinsNumber;


        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Button slowDownButton;


        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private RectTransform coinImage;

        private Camera cam;
        private EventInstance slowDownSnapshot;

        public Vector3 CoinWorldPosition()
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                coinImage,
                coinImage.position,
                cam,
                out var worldPosition
            );
            return worldPosition;
        }


        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Image slowDownImage;

        private CountdownTimer countdownTimer;
        private Stat<float> SlowDownFactor { get; set; }

        private void Awake()
        {
            slowDownSnapshot = RuntimeManager.CreateInstance("snapshot:/SlowMode");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            countdownTimer = new CountdownTimer(3, true);
            countdownTimer.OnTimerStop += SlowDownEnd;
            EventBus<OnWaveFillUpdate>.Register(FillWaveUpdate);
            EventBus<OnWaveUnlocked>.Register(OnWaveUnlocked);
            EventBus<OnGameWin>.Register(OnGameWin);
            EventBus<OnGameLost>.Register(OnGameLost);
            EventBus<OnUpdateClicked>.Register(OnUpdateClicked);
            EventBus<OnCoinsChanged>.Register(OnCoinsChanged);
            var health = WaveUpgradeData.Instance.GetStat<int>("Health");
            health.OnValueChanged += HealthChanged;
            HealthChanged(0, health.Value);
            slowDownButton.onClick.AddListener(SlowDown);
            SlowDownFactor = WaveUpgradeData.Instance.GetStat<float>("SlowDownFactor");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<OnWaveFillUpdate>.Deregister(FillWaveUpdate);
            EventBus<OnWaveUnlocked>.Deregister(OnWaveUnlocked);
            EventBus<OnGameWin>.Deregister(OnGameWin);
            EventBus<OnGameLost>.Deregister(OnGameLost);
            EventBus<OnCoinsChanged>.Deregister(OnCoinsChanged);
            EventBus<OnUpdateClicked>.Deregister(OnUpdateClicked);
            WaveUpgradeData.Instance.GetStat<int>("Health").OnValueChanged -= HealthChanged;
            slowDownButton.onClick.RemoveListener(SlowDown);
        }

        private void Start()
        {
            cam = Camera.main;
        }

        private void HealthChanged(int arg1, int newValue)
        {
            var delta = arg1 - newValue;
            healthNumber.rectTransform.DOKill();
            healthNumber.DOKill();
            healthNumber.rectTransform.DOShakeScale(0.5f, 0.5f).onComplete += () =>
            {
                healthNumber.rectTransform.DOScale(Vector3.one, 0);
            };
            healthNumber.DOColor(delta > 0 ? Color.red : Color.green, 0.5f).onComplete +=
                () => healthNumber.DOColor(Color.white, 0.5f);
            healthNumber.text = $"{newValue}";
        }

        private void FillWaveUpdate(OnWaveFillUpdate eve)
        {
            waveFill.fillAmount = eve.fill;
        }

        private void OnWaveUnlocked(OnWaveUnlocked obj)
        {
            var totalWaves = Manager.Instance.TotalWaves.Value;
            waveNumber.text = $"wave: {obj.waveNumber}/{totalWaves}";
            if (obj.waveNumber <= 1) return;
            countdownTimer.Pause();
            slowDownSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            MenuManager.OpenPanel<WaveUpdateMenu>();
        }


        private void OnGameWin(OnGameWin obj)
        {
            MenuManager.OpenPanel<WinMenu>();
        }

        private void OnGameLost(OnGameLost obj)
        {
            MenuManager.OpenPanel<GameOverMenu>();
        }

        private void SlowDown()
        {
            slowDownButton.interactable = false;
            Time.timeScale = SlowDownFactor;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            countdownTimer.Start();
            slowDownSnapshot.start();
        }

        private void SlowDownEnd()
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            slowDownButton.interactable = true;
            slowDownImage.fillAmount = 1;
            slowDownSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        private void Update()
        {
            if (!countdownTimer.IsRunning) return;
            slowDownImage.fillAmount = 1 - countdownTimer.Progress;
            if (!Mathf.Approximately(Time.timeScale, 1)) return;
            Time.timeScale = SlowDownFactor;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            slowDownSnapshot.start();
        }


        private void OnCoinsChanged(OnCoinsChanged onCoinsChanged)
        {
            coinsNumber.rectTransform.DOKill();
            coinsNumber.rectTransform.DOShakeScale(0.5f, 0.5f).onComplete += () =>
            {
                coinsNumber.rectTransform.DOScale(Vector3.one, 0);
            };
            coinsNumber.text = $"{onCoinsChanged.coins}";
        }

        private void OnUpdateClicked(OnUpdateClicked obj)
        {
            if (slowDownButton.interactable) return;
            Time.timeScale = SlowDownFactor;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            countdownTimer.Resume();
            slowDownSnapshot.start();
        }
    }
}