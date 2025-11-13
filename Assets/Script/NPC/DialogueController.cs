using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private DialogueData dialogueData;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f; // t?c ?? hi?n t?ng ch?

    private int currentIndex = 0;
    private bool isTyping = false;          // ?ang hi?n t?ng ch?
    private bool skipTyping = false;        // ng??i ch?i nh?n E ?? skip
    public bool IsDialogueActive { get; private set; }

    private Coroutine typingCoroutine;

    private void Start()
    {
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
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(dialogueData.dialogueLines[currentIndex]));
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        skipTyping = false;
        dialogueText.text = "";

        foreach (char c in line.ToCharArray())
        {
            if (skipTyping)
            {
                dialogueText.text = line; // hi?n th? h?t luôn
                break;
            }

            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void NextDialogue()
    {
        // N?u ?ang gõ ch? thì skip toàn b? câu
        if (isTyping)
        {
            skipTyping = true;
            return;
        }

        // N?u ?ã hi?n xong câu ? chuy?n sang câu ti?p theo
        currentIndex++;
        ShowDialogue();
    }

    private void EndDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(false);
        IsDialogueActive = false;
        currentIndex = 0;
        isTyping = false;
        skipTyping = false;
    }
}
