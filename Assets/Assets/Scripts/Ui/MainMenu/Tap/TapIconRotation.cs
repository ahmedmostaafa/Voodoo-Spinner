using DG.Tweening;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using UnityEngine;

namespace KabreetGames.BladeSpinner.Ui
{
    public class TapIconRotation : ValidatedMonoBehaviour
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
            if (isOn)
            {
                transform.DOShakeRotation( 0.1f, new Vector3(0, 0, 15f)).SetLoops(1, LoopType.Yoyo);
                ((RectTransform)transform).sizeDelta = new Vector2(220, 220);
            }
            else
            {
                ((RectTransform)transform).sizeDelta = new Vector2(175, 175);
            }
        }
    }
}