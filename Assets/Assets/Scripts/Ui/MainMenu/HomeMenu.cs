using KabreetGames.SceneManagement;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.BladeSpinner.Ui
{
    public class HomeMenu : SupMenuPanel
    {
        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Button startButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            startButton.onClick.AddListener(StartGame);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            startButton.onClick.RemoveListener(StartGame);
        }

        private void StartGame()
        {
            SceneLoadingManager.ReplaceScene(SceneGroupNames.GamePlay, SceneGroupNames.MianMenu, ReplaceSceneMode.UnLoadLoad);
        }
    }
}