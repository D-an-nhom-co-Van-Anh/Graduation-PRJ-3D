using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private DialogueData dialogueData;
    private PlayerController playerController;
    private int currentIndex;
    public bool IsDialogueActive { get; private set; }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        dialoguePanel.SetActive(false);
        IsDialogueActive = false;
    }

    public void StartDialogue()
    {
        currentIndex = 0;
        dialoguePanel.SetActive(true);
        IsDialogueActive = true;
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

    public void NextDialogue()
    {
        currentIndex++;
        ShowDialogue();
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialogueActive = false;
        currentIndex = 0;
    }
}
