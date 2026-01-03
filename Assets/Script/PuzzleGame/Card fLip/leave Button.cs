using UnityEngine;

public class leaveButton : MonoBehaviour
{
    public GameObject introductUI;
    public GameObject gamePlayScene;

    private bool isIntroPlaying;

    private const string INTRO_KEY = "HasPlayedIntro";

    void Start()
    {
        bool hasPlayedIntro = PlayerPrefs.GetInt(INTRO_KEY, 0) == 1;

        if (!hasPlayedIntro)
        {
            PlayIntro(autoPlay: true);
        }
        else
        {
            StartGameplay();
        }
    }

    void Update()
    {
        // Nhấn P để mở lại Intro (KHÔNG ảnh hưởng PlayerPrefs)
        if (!isIntroPlaying && Input.GetKeyDown(KeyCode.P))
        {
            PlayIntro(autoPlay: false);
        }
    }

    // ====== INTRO CONTROL ======

    void PlayIntro(bool autoPlay)
    {
        isIntroPlaying = true;

        introductUI.SetActive(true);
        gamePlayScene.SetActive(false);

        Time.timeScale = 0f;

        // Nếu là intro chạy tự động lần đầu → lưu PlayerPrefs
        if (autoPlay)
        {
            PlayerPrefs.SetInt(INTRO_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    // GỌI TỪ UI BUTTON "EXIT"
    public void CloseIntroByUIButton()
    {
        isIntroPlaying = false;

        introductUI.SetActive(false);
        gamePlayScene.SetActive(true);

        Time.timeScale = 1f;
    }

    void StartGameplay()
    {
        introductUI.SetActive(false);
        gamePlayScene.SetActive(true);

        Time.timeScale = 1f;
        isIntroPlaying = false;
    }
}
