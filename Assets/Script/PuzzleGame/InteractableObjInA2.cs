using System;
using UnityEngine;

public class InteractableObjInA2 : InteractableObj
{
    private bool isTalking = false;
    private bool playerInside = false;

    private DialogueController dialogueController; 
    private PlayerMovementController playerController;

    private enum ObjectType
    {
        DoorQuiz,
        DoorType,
        Teleport
    }

    [SerializeField] ObjectType objectType;
    [SerializeField] GameObject uiTalkingPrompt;

    private void Start()
    {
        dialogueController = GetComponent<DialogueController>();
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (objectType)
            {
                case ObjectType.Teleport:
                    HandleTalkInput();
                    break;

                case ObjectType.DoorQuiz:
                    SceneManager_.Instance.LoadSceneByName("Quiz");
                    break;

                case ObjectType.DoorType:
                    SceneManager_.Instance.LoadSceneByName("Typing");
                    break;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        
        if (objectType != ObjectType.Teleport && uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(true);

        if (objectType == ObjectType.Teleport)
            HandleTalkInput();
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        isTalking = false;

        if (uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(false);
    }


    private void HandleTalkInput()
    {
        isTalking = true;
        playerController.LockMovement();

        if (!dialogueController.IsDialogueActive)
        {
            dialogueController.StartDialogue();
        }
        else
        {
            dialogueController.NextDialogue();
        }
    }
}
