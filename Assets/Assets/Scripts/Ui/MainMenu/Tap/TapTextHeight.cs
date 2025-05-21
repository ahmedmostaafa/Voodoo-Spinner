using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Ui
{
    public class TapTextHeight : ValidatedMonoBehaviour
    {
        [SerializeField, Parent] private Tap tap;
        private void OnEnable()
        {
            if (tap == null) return;
            tap.onValueChanged.AddListener(OnTapValueChanged);
        }
        
        private void OnDisable()
        {
            if (tap == null) return;
            tap.onValueChanged.RemoveListener(OnTapValueChanged);
        }

        private void OnTapValueChanged(bool isOn)
        {
            var rectTransform = (RectTransform)transform;
            var size = rectTransform.sizeDelta;
            size.y = isOn ?  80 :  50;
            rectTransform.sizeDelta = size;
        }
    }
}