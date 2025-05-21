using FMODUnity;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Sounds
{
    [RequireComponent(typeof(Tap))]
    public class TapSounds : ValidatedMonoBehaviour
    {
        [SerializeField] private EventReference tapSound;

        private Tap tap;

        private void OnEnable()
        {
            tap = GetComponent<Tap>();
            tap.onValueChanged.AddListener(PlayTapSound);
        }

        private void OnDisable()
        {
            tap.onValueChanged.RemoveListener(PlayTapSound);
        }

        private void PlayTapSound(bool isOn)
        {
            if (isOn)
            {
                Debug.Log(" PlayTapSound");
                RuntimeManager.PlayOneShot(tapSound);
            }
        }
    }
}