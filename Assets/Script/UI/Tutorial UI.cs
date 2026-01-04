using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public GameObject tutorial_Image;
    void Start()
    {
        // Ẩn UI khi bắt đầu game
        tutorial_Image.SetActive(false);
    }

    void Update()
    {
        // Nhấn Tab để bật / tắt UI
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTutorial();
        }
    }

    // tab + button
    public void ToggleTutorial()
    {
        bool isActive = tutorial_Image.activeSelf;
        tutorial_Image.SetActive(!isActive);
    }

    public void close_Tutorial()
    {
        tutorial_Image.SetActive(false);
    }
}
