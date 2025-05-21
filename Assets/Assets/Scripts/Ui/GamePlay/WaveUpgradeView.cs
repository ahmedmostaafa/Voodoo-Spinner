using DG.Tweening;
using FMODUnity;
using KabreetGames.BladeSpinner.Events;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.BladeSpinner.Ui
{
    public class WaveUpgradeView : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button upgradeButton;

        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private TMP_Text upgradeNameText;

        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private TMP_Text upgradeCostText;    
        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Image upgradeImage;     
        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Image backgroundImage;   
        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Image glowImage;

        private bool clicked;
        private Sequence sequence;
        private WaveUpgrade waveUpgrade;
        
        [SerializeField] private Sprite [] upgradeBackgrounds;
        [SerializeField] private Color [] glowColors;
        
        

        private int upgradeCost;

        private void OnEnable()
        {
            upgradeButton.onClick.AddListener(OnUpdateClicked);
            transform.localScale = Vector3.zero;
            upgradeButton.enabled = true;
            clicked = false;
            upgradeButton.targetGraphic.DOFade(1, 0.1f).SetUpdate(true);
            transform.DOScale(Vector3.one, 0.5f).SetUpdate(true).SetEase(Ease.OutBack);
        }

        private void OnDisable()
        {
            upgradeButton.onClick.RemoveListener(OnUpdateClicked);
        }

        private void OnUpdateClicked()
        {
            // if (Manager.Instance.Coins < upgradeCost) return;
            clicked = true;
            EventBus<OnUpdateClicked>.Rise();
            RuntimeManager.PlayOneShot("event:/Sfx/Upgrade Clicked");
            waveUpgrade.ApplyUpgrade();
            Manager.Instance.Coins -= upgradeCost;
        }

        public void Hide()
        {
            upgradeButton.enabled = false;
            sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InBack, clicked ? 4f : 1.2f));
            sequence.Append(upgradeButton.targetGraphic.DOFade(0, 0.5f).SetEase(Ease.OutExpo));
            sequence.AppendCallback(() => gameObject.SetActive(false));
            sequence.SetUpdate(true);
            sequence.Play();
        }

        public void Init(WaveUpgrade upgrade, int cost, int index)
        {
            backgroundImage.sprite = upgradeBackgrounds[index];
            glowImage.color = glowColors[index];
            waveUpgrade = upgrade;
            upgradeNameText.text = upgrade.Name;
            upgradeCostText.text = cost == 0 ? "Free" : cost.ToString();
            upgradeImage.sprite = upgrade.Icon;
            upgradeCost = cost;
        }
    }
}