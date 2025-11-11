using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private DialogueData dialogueData;

    private int currentIndex = 0;
    private bool isDialogueActive = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            NextDialogue();
        }
    }

    public void StartDialogue()
    {
        currentIndex = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        if (currentIndex < dialogueData.dialogueLines.Count)
        {
            dialogueText.text = dialogueData.dialogueLines[currentIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    private void NextDialogue()
    {
        currentIndex++;
        ShowDialogue();
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}
