using UnityEngine;

public class LeaveButton : MonoBehaviour
{
    // UI References
    public GameObject tutorialPanel;
    private const string TUTORIAL_KEY = "TutorialShown";
    private bool isTutorialOpen = false;

    void Start()
    {
        // Lần đầu chơi game auto show tutorial
        if (!PlayerPrefs.HasKey(TUTORIAL_KEY))
        {
            OpenTutorial(true);

            PlayerPrefs.SetInt(TUTORIAL_KEY, 1);
            PlayerPrefs.Save();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Nhấn Tab để mở / đóng tutorial
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isTutorialOpen)
                CloseTutorial();
            else
                OpenTutorial(false);
        }
    }

    // Mở tutorial
    void OpenTutorial(bool firstTime)
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
        isTutorialOpen = true;
    }

    // Nút X hoặc nhấn P lần nữa
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        isTutorialOpen = false;
    }
}
