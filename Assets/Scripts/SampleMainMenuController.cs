using UnityEngine;
using DG.Tweening;
using Aviss;

public class SampleMainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject helpMenuScreen;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        mainMenuScreen.SetActive(true);
        helpMenuScreen.SetActive(false);

    }

    public void Start()
    {
        AudioController.Instance.Play("Music");
    }

    public void Play()
    {
        SceneController.Instance.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Help()
    {
        helpMenuScreen.SetActive(true);

        mainMenuScreen.GetComponent<RectTransform>().DOAnchorPosX(800, 0.4f).OnComplete(() =>
        {
            mainMenuScreen.SetActive(false);
            helpMenuScreen.GetComponent<RectTransform>().DOAnchorPosX(0, 0.4f);
        });
    }

    public void Back()
    {
        mainMenuScreen.SetActive(true);

        helpMenuScreen.GetComponent<RectTransform>().DOAnchorPosX(800, 0.4f).OnComplete(() =>
        {
            helpMenuScreen.SetActive(false);
            mainMenuScreen.GetComponent<RectTransform>().DOAnchorPosX(0, 0.4f);
        });
    }
}
