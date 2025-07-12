using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Aviss
{
    public class SceneController : PersistentSingleton<SceneController>
    {
        public enum TransitionDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            CENTER
        }

        [System.Serializable]
        public struct TransitionPrefs
        {
            public bool UP;
            public bool DOWN;
            public bool LEFT;
            public bool RIGHT;
            public bool CENTER;

            public List<TransitionDirection> GetDirections()
            {
                List<TransitionDirection> directions = new List<TransitionDirection>();

                if (UP)
                    directions.Add(TransitionDirection.UP);
                if (DOWN)
                    directions.Add(TransitionDirection.DOWN);
                if (LEFT)
                    directions.Add(TransitionDirection.LEFT);
                if (RIGHT)
                    directions.Add(TransitionDirection.RIGHT);
                if (CENTER)
                    directions.Add(TransitionDirection.CENTER);

                return directions;
            }
        }

        [Header("Transition")]
        [SerializeField] private TransitionPrefs inTransitionPrefs;
        [SerializeField] private TransitionPrefs outTransitionPrefs;
        private List<TransitionDirection> inTransitionDirections;
        private List<TransitionDirection> outTransitionDirections;

        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private RectTransform veilRect;

        [Header("Misc")]
        [SerializeField] private bool returnOnEscape = true;
        private bool isSceneLoadInProgress = true;

        private new void Awake()
        {
            base.Awake();
            inTransitionDirections = inTransitionPrefs.GetDirections();
            outTransitionDirections = outTransitionPrefs.GetDirections();
        }

        private void OnEnable()
        {
            // Suscribirse al evento cuando la escena ha sido cargada
            SceneManager.sceneLoaded += OnSceneLoaded;
            veilRect.DOKill(true);
        }

        private void OnDisable()
        {
            // Asegurarse de desuscribirse cuando el objeto se desactive o sea destruido
            SceneManager.sceneLoaded -= OnSceneLoaded;
            DOTween.Clear();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            DOTween.Clear();
            SlideOut();
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && returnOnEscape && !isSceneLoadInProgress)
            {
                LoadScene(0);
            }
        }

        public void LoadScene(int index)
        {
            if (!isSceneLoadInProgress)
            {
                if (index >= 0)
                {
                    isSceneLoadInProgress = true;
                    SlideIn();
                    StartCoroutine(LoadAsyncScene(index));
                }
                else
                {
                    Debug.LogError("The scene index can't be less than 0");
                }
            }
        }

        public void SlideIn(TweenCallback onComplete = null)
        {
            if (inTransitionDirections.Count == 0)
            {
                return; // No directions selected, so return early.
            }

            // Select a random direction from the ones that have been chosen.
            var random = new System.Random();
            var randomDirection = inTransitionDirections[random.Next(inTransitionDirections.Count)];

            Vector2 startPosition;

            switch (randomDirection)
            {
                case TransitionDirection.UP:
                    startPosition = new Vector2(0.0f, -1000.0f);
                    break;
                case TransitionDirection.LEFT:
                    startPosition = new Vector2(1000.0f, 0.0f);
                    break;
                case TransitionDirection.RIGHT:
                    startPosition = new Vector2(-1000.0f, 0.0f);
                    break;
                case TransitionDirection.DOWN:
                    startPosition = new Vector2(0.0f, 1000.0f);
                    break;
                case TransitionDirection.CENTER:
                    veilRect.localScale = Vector2.zero;
                    veilRect.anchoredPosition = Vector2.zero;
                    veilRect.DOScale(1.0f, transitionDuration).OnComplete(onComplete ?? (() => { }));
                    return;
                default:
                    return;
            }

            veilRect.anchoredPosition = startPosition;
            veilRect.localScale = Vector2.one;
            veilRect.DOAnchorPos(Vector2.zero, transitionDuration).OnComplete(onComplete ?? (() => { }));
        }

        public void SlideOut()
        {
            if (outTransitionDirections.Count == 0)
            {
                return; // No directions selected, so return early.
            }

            // Select a random direction from the ones that have been chosen.
            var random = new System.Random();
            var randomDirection = outTransitionDirections[random.Next(outTransitionDirections.Count)];

            Vector2 endPosition;
            veilRect.localScale = Vector2.one;
            veilRect.anchoredPosition = Vector2.zero;

            switch (randomDirection)
            {
                case TransitionDirection.UP:
                    endPosition = new Vector2(0.0f, 1000.0f);
                    break;
                case TransitionDirection.LEFT:
                    endPosition = new Vector2(-1000.0f, 0.0f);
                    break;
                case TransitionDirection.RIGHT:
                    endPosition = new Vector2(1000.0f, 0.0f);
                    break;
                case TransitionDirection.DOWN:
                    endPosition = new Vector2(0.0f, -1000.0f);
                    break;
                case TransitionDirection.CENTER:
                    veilRect.DOScale(0.0f, transitionDuration).OnComplete(() => isSceneLoadInProgress = false);
                    return;
                default:
                    return;
            }

            veilRect.DOAnchorPos(endPosition, transitionDuration).OnComplete(() => isSceneLoadInProgress = false);
        }

        private IEnumerator LoadAsyncScene(int sceneIndex)
        {
            yield return new WaitForSeconds(transitionDuration);

            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneIndex);

            while (!loading.isDone)
            {
                yield return null;
            }
        }
    }
}
