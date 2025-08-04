using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public float minimumMouseSensitivity = 5f;
    public float maximumMouseSensitivity = 150f;
    public float defaultMouseSensitivity = 20f;
    public float transitionDuration = 0.5f;
    public Scrollbar scrollbar;
    public PlayerController playerController;


    private RectTransform rect;
    private Tween currentTween;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
        playerController.mouseSensitivity = defaultMouseSensitivity;
        scrollbar.value = Mathf.InverseLerp(minimumMouseSensitivity, maximumMouseSensitivity, playerController.mouseSensitivity);
    }

    void Update()
    {
        if (GameManager.Instance.IsNamingFood || GameManager.Instance.isWritingFoodName) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        GameManager.Instance.TogglePause();

        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(true);
        }

        if (GameManager.Instance.IsPaused)
        {
            currentTween = rect.DOAnchorPosY(0, transitionDuration).SetEase(Ease.OutCubic);
        }
        else
        {
            currentTween = rect.DOAnchorPosY(rect.rect.height, transitionDuration).SetEase(Ease.InCubic);
        }
    }

    private void OnScrollbarValueChanged(float value)
    {
        // Aquí puedes manejar el evento de cambio del scrollbar si es necesario
        Debug.Log("Scrollbar value changed: " + value);

        playerController.mouseSensitivity = Mathf.Lerp(
            minimumMouseSensitivity,
            maximumMouseSensitivity,
            value
        );
    }
}
