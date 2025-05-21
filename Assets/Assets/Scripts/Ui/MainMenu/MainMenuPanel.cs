using KabreetGames.SceneManagement;
using KabreetGames.UiSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KabreetGames.BladeSpinner.Assets.Scripts.Ui.MainMenu
{
    public class MainMenuPanel : MenuPanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Application.targetFrameRate = 60;
            if (PlayerPrefs.HasKey("FirstTime")) return;
            LoadTutorial();
            PlayerPrefs.SetInt("FirstTime", 1);
        }

        private void LoadTutorial()
        {
            SceneLoadingManager.LoadScene(SceneGroupNames.Tutorial, loadSceneMode: LoadSceneMode.Single);
        }


#if UNITY_EDITOR

        [UnityEditor.MenuItem("Kabreet Games/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

#endif
    }
}