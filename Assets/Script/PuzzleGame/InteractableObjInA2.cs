using System;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class InteractableObjInA2 : InteractableObj
{
    private bool isTalking = false;
    private bool playerInZone = false;

    private DialogueController dialogueController; 
    private PlayerMovementController playerController;

    private enum ObjectType
    {
        DoorCard,
        DoorType,
        Teleport
    }

    [SerializeField] ObjectType objectType;
    [SerializeField] GameObject uiTalkingPrompt;

    private void Start()
    {
        dialogueController = GetComponent<DialogueController>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        
        if (playerInZone)
        {
            if (Input.GetKeyDown(KeyCode.E))
        
                switch (objectType)
                {

                    case ObjectType.DoorCard:
                        SceneManager_.Instance.LoadSceneByName("Card");
                        break;

                    case ObjectType.DoorType:
                        SceneManager_.Instance.LoadSceneByName("Typing");
                        break;
                }

            if (objectType == ObjectType.Teleport)
            {
                HandleTalkInput();
            }
        }
        
        
        
    }
    


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = true;
        
        if (objectType != ObjectType.Teleport && uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(true);

        if (objectType == ObjectType.Teleport)
            HandleTalkInput();
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInZone = false;
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
        else if(dialogueController.IsDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            dialogueController.NextDialogue();
        }
    }
}
