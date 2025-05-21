using KabreetGames.UiSystem;
using DG.Tweening;
using KabreetGames.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace KabreetGames.BladeSpinner.Ui
{
    public class WinMenu : MenuPanel
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        protected override void OnEnable()
        {
            base.OnEnable();
            restartButton.onClick.AddListener(OnRestartClicked);
            mainMenuButton.onClick.AddListener(ToMainMenuClicked);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            restartButton.onClick.RemoveListener(OnRestartClicked);
            mainMenuButton.onClick.RemoveListener(ToMainMenuClicked);
        }

        private void OnRestartClicked()
        {
            WaveUpgradeData.Instance.ResetStats();
            Time.timeScale = 1;
            SceneLoadingManager.ReplaceScene(SceneGroupNames.MianMenu, SceneGroupNames.GamePlay);
        }
        
        private void ToMainMenuClicked()
        {
            // WaveUpgradeData.Instance.ResetStats();
            // Time.timeScale = 1;
            // SceneLoadingManager.ReplaceScene(SceneGroupNames.MianMenu, SceneGroupNames.MainMenu);
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
            Time.timeScale = 0;
            Group.DOFade(1, 0.5f).SetUpdate(true);
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
            Time.timeScale = 1;
            Group.DOFade(0, 0.5f).SetUpdate(true);
        }
    }
}