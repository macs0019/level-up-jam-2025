using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Aviss
{
    public class TutorialController : MonoBehaviour
    {
        [System.Serializable]
        public struct Tutorial
        {
            [TextArea(3, 10)]
            public List<string> textList;
        }

        [SerializeField] private List<Tutorial> tutorialFrames;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField, Range(0.0f, 5.0f)] private float timeBeforeStartTutorial = 1.0f;
        [SerializeField, Range(0.5f, 2.0f)] private float timeBetweenTransition = 1.0f;

        [Header("Player Prefs")]
        [SerializeField] public bool savePlayerPrefs = true;
        [SerializeField] private string playerPrefsKey = "SampleTutorial";

        [Header("Position")]
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 endPosition;
        private RectTransform tutorialRect;

        [Header("Typing Speed")]
        [SerializeField, Range(0.03f, 0.1f)] private float timePerCharacter = 0.05f;
        [SerializeField, Range(2.0f, 4.0f)] private float timePerCharacterMultiplier = 3.0f;
        private float waitTimePerCharacter;

        public delegate void TutorialEndEventHandler();
        public event TutorialEndEventHandler OnTutorialEnd;

        private int currentFrame;
        private int currentFrameText;
        private bool canContinue;

        private void Awake()
        {
            tutorialRect = GetComponent<RectTransform>();
            tutorialRect.anchoredPosition = startPosition;

            if (savePlayerPrefs)
            {
                if (!PlayerPrefs.HasKey(playerPrefsKey))
                {
                    PlayerPrefs.SetInt(playerPrefsKey, 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        void Start()
        {
            waitTimePerCharacter = timePerCharacter;

            canContinue = false;
            currentFrame = 0;
            currentFrameText = 0;
            tutorialText.text = "";

            DOTween.Sequence()
                .AppendInterval(timeBeforeStartTutorial)
                .AppendCallback(StartCurrentFrame);
        }

        public void Continue()
        {
            if (canContinue)
            {
                NextTutorial();
            }
            else
            {
                waitTimePerCharacter = timePerCharacter / timePerCharacterMultiplier;
            }
        }

        private void StartCurrentFrame()
        {
            if (currentFrame < tutorialFrames.Count)
            {
                tutorialRect.DOAnchorPos(endPosition, timeBetweenTransition).SetEase(Ease.InOutSine).OnComplete(() => WriteText());
            }
            else
            {
                // Event called when the tutorial ends!
                if (OnTutorialEnd != null)
                {
                    OnTutorialEnd();
                }
            }
        }

        private void NextTutorial()
        {
            if (currentFrame < tutorialFrames.Count)
            {
                if (currentFrameText < tutorialFrames[currentFrame].textList.Count)
                {
                    WriteText();
                }
                else
                {
                    ChangeFrame();
                }
            }
        }

        private void WriteText()
        {
            canContinue = false;
            tutorialText.text = "";

            string currentText = tutorialFrames[currentFrame].textList[currentFrameText];
            float totalTime = currentText.Length * waitTimePerCharacter;

            DOTween.To(() => tutorialText.text, x => tutorialText.text = x, currentText, totalTime)
                .OnComplete(() =>
                {
                    waitTimePerCharacter = timePerCharacter;
                    currentFrameText++;

                    canContinue = true;
                });
        }

        private void ChangeFrame()
        {
            canContinue = false;

            tutorialRect.DOAnchorPos(startPosition, timeBetweenTransition).SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    // Reset the text when the slide is out of the screen
                    tutorialText.text = "";

                    currentFrameText = 0;
                    currentFrame++;

                    StartCurrentFrame();
                });
        }
    }
}
