
using System.Collections;
using UnityEngine;

public class InteractableObjReal : InteractableObj
{
    private bool isTalking = false;
    private bool playerInZone = false;
    private BoxCollider  triggerZone;
    private DialogueController dialogueController; 
    private PlayerMovementController playerController;
    private TeleportA2 _teleportA2;
    private enum ObjectType
    {
        DoorCard,
        DoorType,
        Teleport,
        DoorA2,
        Shop,
    }

    [SerializeField] ObjectType objectType;
    [SerializeField] GameObject uiTalkingPrompt;

    private void Start()
    {
        _teleportA2 = GetComponent<TeleportA2>();
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
                    case ObjectType.DoorA2:
                        _teleportA2.Teleport();
                        StartCoroutine(LockMovement());
                        GameManager_.Instance.PlayTimeline();
                        break;
                  
                }

            if (objectType == ObjectType.Teleport)
            {
                HandleTalkInput();
            }
        }
        
        
        
    }

    IEnumerator LockMovement()
    {
        playerController.LockMovement();
        yield return new WaitForSeconds(2.0f);
        playerController.UnlockMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        if (objectType != ObjectType.Teleport && objectType != ObjectType.Shop && uiTalkingPrompt != null)
        {
            
            uiTalkingPrompt.SetActive(true);
            playerInZone = true;
        }
        else if (objectType == ObjectType.Shop && QuestManager.Instance.CheckQuest("Quest4Info", QuestState.FINISHED))
        {
            uiTalkingPrompt.SetActive(true);
            playerInZone = true;
        }
        else if (objectType == ObjectType.Teleport)
        {
            HandleTalkInput();
            playerInZone = true; 
        }
            
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
