using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KabreetGames.BladeSpinner.Events;
using KabreetGames.BladeSpinner.TowerSystem;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.BladeSpinner
{
    public class Tutorial : ValidatedMonoBehaviour
    {
        [SerializeField] private RectTransform character;
        [SerializeField] private TMP_Text text;

        [SerializeField] private RectTransform hand;

        [SerializeField] private Image selectImage;

        [SerializeField] private RectTransform slowDownButton;
        [SerializeField] private GameObject spinner;
        [SerializeField] private TowerSpawner towerSpawner;

        private Material material;

        private TutorialSequence tutorialSequence;
        private static readonly int ScreenPos = Shader.PropertyToID("_ScreenPos");

        private bool slowDownClicked;
        private bool draggedSpinner;


        private void OnEnable()
        {
            EventBus<OnSpinnerAreaChange>.Register((() => draggedSpinner = true));
        }

        private IEnumerator Start()
        {
            material = selectImage.material;
            character.gameObject.SetActive(false);
            text.transform.parent.gameObject.SetActive(false);
            hand.gameObject.SetActive(false);
            selectImage.gameObject.SetActive(false);

            tutorialSequence = new TutorialSequence();
            tutorialSequence.AddMessage("Hi there! Welcome to the Shadow Ninja!",
                () => Input.GetMouseButtonDown(0), false, timeScale: 0);

            tutorialSequence.AddMessage("I am trying to protect my home and I need your help!",
                () => Input.GetMouseButtonDown(0), false, timeScale: 0);

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                slowDownButton,
                slowDownButton.position, null, out var pos);

            tutorialSequence.AddMessage("Use the slow down button to slow down time!",
                () => slowDownClicked, true, null, 1, pos);

            tutorialSequence.AddMessage("Great! Now drag the spinner to the best area to hit the most towers!",
                () => draggedSpinner, false, null, 1, GetSpinnerPosition());

            tutorialSequence.AddMessage("wow! Now try to hit the tower!",
                () => Input.GetMouseButtonDown(0), false, StartSpawner, 1, GetSpinnerPosition());

            yield return new WaitForEndOfFrame();
            StartSequence();
        }

        private void StartSpawner()
        {
            towerSpawner.enabled = true;
        }

        private Vector3 GetSpinnerPosition()
        {
            return spinner.transform.position;
        }

        private void StartSequence()
        {
            MoveToNextMessage();
        }

        private void Update()
        {
            if (!tutorialSequence.Running || !tutorialSequence.CheckCondition()) return;
            MoveToNextMessage();
        }

        private void MoveToNextMessage()
        {
            var currentMessage = tutorialSequence.NextMessage();
            if (currentMessage == null)
            {
#if UNITY_EDITOR
                Debug.Log("End of sequence reached");
#endif
                character.gameObject.SetActive(false);
                text.transform.parent.gameObject.SetActive(false);
                hand.gameObject.SetActive(false);
                selectImage.gameObject.SetActive(false);
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f;
                return;
            }

            character.gameObject.SetActive(currentMessage.ShowMessage);
            text.transform.parent.gameObject.SetActive(currentMessage.ShowMessage);
            Time.timeScale = currentMessage.timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            currentMessage.startAction?.Invoke();
            if (currentMessage.ShowMessage)
            {
                text.transform.parent.DOScaleX(0, 0).SetUpdate(true);
                text.text = string.Empty;
                character.DOAnchorPos(new Vector2(-250, -457), 0.0f).SetUpdate(true).onComplete += () =>
                {
                    character.DOAnchorPos(new Vector2(0, -457), 0.5f).SetEase(Ease.OutBack).SetUpdate(true)
                            .onComplete +=
                        () =>
                        {
                            text.transform.parent.DOScaleX(1, 0.5f).SetEase(Ease.OutBack).SetUpdate(true)
                                .onComplete += () => { text.text = currentMessage.message; };
                        };
                };
            }

            hand.gameObject.SetActive(currentMessage.showHands);
            selectImage.gameObject.SetActive(currentMessage.showHands);
            if (!currentMessage.showHands) return;
            var pos = currentMessage.handPos + new Vector3(-110.2f, 54.2f, 0);
            var screenPos = new Vector2(currentMessage.handPos.x / Screen.width,
                currentMessage.handPos.y / Screen.height);
            hand.anchoredPosition = pos;
            material.SetVector(ScreenPos, screenPos);
        }


        public void SlowDownClicked()
        {
            slowDownClicked = true;
        }
    }


    [Serializable]
    public class TutorialMessage
    {
        public string message;
        public Func<bool> checkCondition;
        public Action startAction;
        public bool showHands;
        public Vector3 handPos;
        public float timeScale;
        public bool ShowMessage => !message.IsNullOrWhitespace();

        public TutorialMessage(string message, Func<bool> checkCondition, bool showHands, float timeScale,
            Action startAction, Vector3 handPos = default)
        {
            this.message = message;
            this.checkCondition = checkCondition;
            this.showHands = showHands;
            this.timeScale = timeScale;
            this.startAction = startAction;
            this.handPos = handPos;
        }
    }

    public class TutorialSequence
    {
        private readonly List<TutorialMessage> messages = new();
        private int currentMessageIndex = -1;

        private TutorialMessage CurrentMessage => messages[currentMessageIndex];
        public bool Running => currentMessageIndex < messages.Count;

        public TutorialMessage NextMessage()
        {
            currentMessageIndex++;
            return currentMessageIndex >= messages.Count ? null : CurrentMessage;
        }

        public void Reset() => currentMessageIndex = -1;

        public bool CheckCondition()
        {
            if (currentMessageIndex < 0 || currentMessageIndex >= messages.Count) return false;
            return CurrentMessage.checkCondition();
        }

        public void AddMessage(string clickTheButtonToStartTheGame, Func<bool> func, bool b,
            Action startAction = null,
            float timeScale = 1,
            Vector3 vector3 = default)
        {
            messages.Add(new TutorialMessage(clickTheButtonToStartTheGame, func, b, timeScale, startAction, vector3));
        }
    }
}