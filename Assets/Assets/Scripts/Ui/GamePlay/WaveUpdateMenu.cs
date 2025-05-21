using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FMODUnity;
using KabreetGames.BladeSpinner.Events;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace KabreetGames.BladeSpinner.Ui
{
    public class WaveUpdateMenu : MenuPanel
    {
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private WaveUpgradeView upgradeViewPrefab;

        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Transform upgradeViewParent;

        private ObjectPool<WaveUpgradeView> upgradeViewPool;

        private readonly List<WaveUpgradeView> upgradeViews = new();

        private const int MaxUpgradeView = 3;

        private void Awake()
        {
            upgradeViewPool = new ObjectPool<WaveUpgradeView>(
                () => Instantiate(upgradeViewPrefab, upgradeViewParent),
                view => view.gameObject.SetActive(true),
                view => view.Hide(),
                Destroy);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus<OnUpdateClicked>.Register(OnUpdateClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<OnUpdateClicked>.Deregister(OnUpdateClicked);
        }

        private void OnUpdateClicked(OnUpdateClicked obj)
        {
            MenuManager.Back();
        }

        protected override void ShowEffect()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Group.alpha = 1;
                return;
            }
#endif
            coinsText.text = Manager.Instance.Coins.ToString();
            CreateUpdateView();
            Time.timeScale = 0;
            Group.DOFade(1, 0.5f).SetUpdate(true);
            RuntimeManager.PlayOneShot("event:/Sfx/ApgradeMenu Open");
        }

        private void CreateUpdateView()
        {
            var totalCoins = Manager.Instance.Coins * 1.5f;
            var upgrades = WaveUpgradeData.Instance.GetUniqueRandomWaveUpgrade(MaxUpgradeView);
            const int maxInt = 3; // Change this to your desired max value
            var numbers = new List<int>();
            for (var i = 0; i < maxInt; i++)
            {
                numbers.Add(i);
            }
            Shuffle(numbers);
            for (var i = 0; i < MaxUpgradeView; i++)
            {
                var cost = Random.Range(0, totalCoins + 1);
                var view = upgradeViewPool.Get();
                upgradeViews.Add(view);
                view.Init(upgrades[i], Mathf.RoundToInt(cost), numbers[i]);
            }
        }

        private static void Shuffle(List<int> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        private void HideUpdateView()
        {
            foreach (var view in upgradeViews)
            {
                upgradeViewPool.Release(view);
            }

            upgradeViews.Clear();
        }

        protected override void HideEffect()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Group.alpha = 0;
                return;
            }
#endif
            HideUpdateView();
            Time.timeScale = 1;
            Group.DOFade(0, 0.5f).SetUpdate(true);
        }
    }
}