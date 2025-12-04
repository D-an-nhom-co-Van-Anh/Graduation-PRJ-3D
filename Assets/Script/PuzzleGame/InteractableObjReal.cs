using System;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class InteractableObjReal : InteractableObj
{
    private bool isTalking = false;
    private bool playerInZone = false;
    private BoxCollider  triggerZone;
    private DialogueController dialogueController; 
    private PlayerMovementController playerController;
    private enum ObjectType
    {
        DoorCard,
        DoorType,
        Teleport,
        Shop,
    }

    [SerializeField] ObjectType objectType;
    [SerializeField] GameObject uiTalkingPrompt;

    private void Start()
    {
        
        triggerZone=GetComponent<BoxCollider>();
        dialogueController = GetComponent<DialogueController>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerMovementController>();
        if (dialogueController != null)
        {
            dialogueController.OnDialogueFinished += OnDialogueFinishedHandler;
        }
    }

    private void Update()
    {
        
        if (playerInZone)
        {
            if (Input.GetKeyDown(KeyCode.E))
        
                switch (objectType)
                {

                    case ObjectType.DoorCard :
                        SceneManager_.Instance.LoadSceneByName("CardFlip");
                        break;

                    case ObjectType.DoorType:
                        SceneManager_.Instance.LoadSceneByName("Typing");
                        break;
                    case ObjectType.Shop:
                        GameManager_.Instance.EnableUIShop();
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
        
        if (objectType != ObjectType.Teleport&& objectType!=ObjectType.Shop && uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(true);
        
        else if(objectType == ObjectType.Shop )
            uiTalkingPrompt.SetActive(true);
        
        else if (objectType == ObjectType.Teleport)
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

    private void OnDialogueFinishedHandler()
    {
        isTalking = false;
        triggerZone.enabled = false;
        playerInZone = false;
        uiTalkingPrompt?.SetActive(false);

        playerController?.UnlockMovement();
    }
}
