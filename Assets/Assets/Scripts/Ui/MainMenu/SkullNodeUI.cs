using System;
using System.Collections.Generic;
using System.Linq;
using KabreetGames.SceneReferences;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.BladeSpinner.Ui
{
    public class SkullNodeUI : MonoBehaviour
    {
        [SerializeField, SerializeReference] private StatWaveUpgrade upgrade;
        [SerializeField, ReadOnly] private string id;
        public string nodeName;
        public List<SkullNodeUI> prerequisites;
        public bool unlocked;

        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private TMP_Text nodeNameText;

        [SerializeField, Child(Flag.Editable | Flag.ShowInInspector)]
        private Image iconImage;

        [SerializeField, Self] private Button button;
        [SerializeField, Self] private CanvasGroup group;

        private void Awake()
        {
            nodeNameText.text = nodeName;

                // iconImage.sprite = upgrade.;

            var valid = prerequisites.All(p => p.unlocked);
            group.interactable = valid;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnNodeClicked);
            unlocked = PlayerPrefs.GetInt("SkullNode" + id, 0) == 1;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnNodeClicked);
            PlayerPrefs.SetInt("SkullNode" + id, unlocked ? 1 : 0);
        }

        private void OnNodeClicked()
        {
            if (unlocked) return;

            Unlock();
        }

        private void Unlock()
        {
            unlocked = true;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString();
        }
    }
}