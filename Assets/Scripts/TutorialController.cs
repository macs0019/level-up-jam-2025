using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

namespace Aviss
{
    public class TutorialController : PersistentSingleton<TutorialController>
    {
        [System.Serializable]
        public struct Tutorial
        {
            public bool skipWithSpace;

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

        [SerializeField] private Transform spaceBarTransform;

        public delegate void TutorialEndEventHandler();
        public event TutorialEndEventHandler OnTutorialEnd;

        private int currentFrame;
        private int currentFrameText;
        private bool canContinue;

        protected override void Awake()
        {
            base.Awake();

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
                .AppendCallback(StartCurrentFrame)
                .SetUpdate(true);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (tutorialFrames[currentFrame].skipWithSpace && currentFrameText < tutorialFrames[currentFrame].textList.Count)
                {
                    spaceBarTransform.DOKill(true);
                    spaceBarTransform.DOPunchScale(Vector3.one / 10f, 0.2f);
                    Continue();
                }
            }
        }

        public void Continue()
        {
            if (canContinue)
            {
                Debug.Log(currentFrameText);
                NextTutorial();
            }
            else
            {
                waitTimePerCharacter = timePerCharacter / timePerCharacterMultiplier;
            }
        }

        public void SkipFrame()
        {
            spaceBarTransform.DOScale(0f, 0.4f).OnComplete(() =>
            {
                spaceBarTransform.gameObject.SetActive(false);
            });

            ChangeFrame();
        }

        private void StartCurrentFrame()
        {
            if (currentFrame < tutorialFrames.Count)
            {
                tutorialRect.DOAnchorPos(endPosition, timeBetweenTransition).SetUpdate(true).SetEase(Ease.InOutSine).OnComplete(() => WriteText());
            }
            else
            {
                tutorialRect.DOAnchorPos(startPosition, timeBetweenTransition).SetUpdate(true).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    // Event called when the tutorial ends!
                    if (OnTutorialEnd != null)
                    {
                        OnTutorialEnd();
                    }
                });
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

            Debug.Log("WriteText: " + currentFrameText);

            string currentText = tutorialFrames[currentFrame].textList[currentFrameText];
            float totalTime = currentText.Length * waitTimePerCharacter;

            AudioController.Instance.Play("Boss talk");
            DOTween.To(() => tutorialText.text, x => tutorialText.text = x, currentText, totalTime)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    AudioController.Instance.Stop("Boss talk");
                    waitTimePerCharacter = timePerCharacter;
                    currentFrameText++;

                    canContinue = true;
                });
        }

        private void ChangeFrame()
        {
            canContinue = false;

            tutorialRect.DOAnchorPos(startPosition, timeBetweenTransition).SetEase(Ease.InOutSine)
                .SetUpdate(true)
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
