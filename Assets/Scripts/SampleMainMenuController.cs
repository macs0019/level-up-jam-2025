using UnityEngine;
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
        mainMenuScreen.SetActive(false);
        helpMenuScreen.SetActive(true);
    }

    public void Back()
    {
        helpMenuScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
}
