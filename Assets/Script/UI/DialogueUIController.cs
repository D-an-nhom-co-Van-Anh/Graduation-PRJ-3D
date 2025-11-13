using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text dialogueText;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;
    public AudioSource typingSound; // optional

    public bool IsTyping { get; private set; }

    private string currentLine = "";
    private Coroutine typingCoroutine;

    void Start()
    {
        ShowDialoguePanel(false);
    }

    public void ShowDialoguePanel(bool state)
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(state);
    }

    public void DisplayLine(string line)
    {
        currentLine = line;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line)
    {
        IsTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            if (typingSound != null)
                typingSound.PlayOneShot(typingSound.clip);
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }

    public void ShowFullLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentLine;
        IsTyping = false;
    }
}
