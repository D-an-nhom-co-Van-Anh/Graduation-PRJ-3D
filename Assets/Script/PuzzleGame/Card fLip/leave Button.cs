using UnityEngine;

public class leaveButton : MonoBehaviour
{
    [Header("UI References")]
    public GameObject introUI;
    public GameObject gameplayUI;

    private bool isIntroPlaying;

    private const string INTRO_KEY = "HasPlayedIntro";

    void Start()
    {
        bool hasPlayedIntro = PlayerPrefs.GetInt(INTRO_KEY, 0) == 1;

        if (!hasPlayedIntro)
        {
            ShowIntro(true);
        }
        else
        {
            ShowGameplay();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isIntroPlaying)
            {
                CloseIntro();
            }
            else
            {
                ShowIntro(false);
            }
        }
    }

    // INTRO

    public void ShowIntro(bool autoPlay)
    {
        isIntroPlaying = true;

        introUI.SetActive(true);
        gameplayUI.SetActive(false);

        PauseGame();

        if (autoPlay)
        {
            PlayerPrefs.SetInt(INTRO_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    public void CloseIntro()
    {
        isIntroPlaying = false;

        introUI.SetActive(false);
        gameplayUI.SetActive(true);

        ResumeGame();
    }
    // GAMEPLAY SHOW
    private void ShowGameplay()
    {
        introUI.SetActive(false);
        gameplayUI.SetActive(true);

        isIntroPlaying = false;
        ResumeGame();
    }

    // TIME CONTROL
    // ko dùng coroutine để tránh rắc rối với unscaledDeltaTime

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}

