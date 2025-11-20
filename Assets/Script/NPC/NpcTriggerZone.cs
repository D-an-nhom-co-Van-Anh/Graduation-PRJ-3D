using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class NpcTriggerZone : MonoBehaviour
{
    [Header("References")]
    public Collider triggerZone;              // Box Collider vùng nói chuyện
    public GameObject uiTalkingPrompt;        // UI "Press E to Talk"
    public NpcController npcController;       // Script điều khiển NPC
    public Transform npcTransform;            // Transform của NPC
    private Transform playerTransform;        // Player (tự tìm bằng tag)

    [Header("Settings")]
    public float rotationSpeed = 3f;          // Tốc độ xoay NPC
    public float rotationDelay = 5f;          // Thời gian chờ giữa các lần xoay (giây)

    private bool playerInZone = false;
    private bool isTalking = false;
    private float nextRotationTime = 0f;      // Thời điểm được xoay lần kế tiếp
  
    public DialogueController dialogueController;

    public PlayerMovementController playerController;
    
    private void Reset()
    {
        triggerZone = GetComponent<Collider>();
        if (triggerZone != null)
            triggerZone.isTrigger = true;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj.transform;
        dialogueController = GetComponent<DialogueController>();
        playerController = playerTransform.GetComponent<PlayerMovementController>();
        
        if (uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(false);

        if (npcTransform == null && npcController != null)
            npcTransform = npcController.transform;
    
        if (dialogueController != null)
        {
            dialogueController.OnDialogueFinished += OnDialogueFinishedHandler;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            if (uiTalkingPrompt != null)
                uiTalkingPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            isTalking = false;

            if (uiTalkingPrompt != null)
                uiTalkingPrompt.SetActive(false);

            npcController?.SetTalking(false);
        }
    }

    private void Update()
    {
        if (!triggerZone.enabled) return;

        // Ấn E để nói chuyện
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            HandleTalkInput();
        }

        // Xoay NPC nhìn Player
        if (isTalking && playerInZone)
        {
            playerController.LockMovement();

            if (Time.time >= nextRotationTime)
            {
                RotateTowardPlayer();
                nextRotationTime = Time.time + rotationDelay;
            }
        }
        else
        {
            playerController.UnlockMovement();
        }
    }

    private void HandleTalkInput()
    {
        isTalking = true;
        playerController.LockMovement();

        if (!dialogueController.IsDialogueActive)
        {
            dialogueController.StartDialogue();
            npcController?.SetTalking(true);
        }
        else
        {
            dialogueController.NextDialogue();
        }

        uiTalkingPrompt?.SetActive(false);
    }

    private void RotateTowardPlayer()
    {
        Vector3 direction = (playerTransform.position - npcTransform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            StartCoroutine(SmoothRotate(targetRotation));
        }
    }

    private System.Collections.IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        float t = 0f;
        Quaternion startRotation = npcTransform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            npcTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }
    }
    public bool isPlayerInZone()
    {
        if (playerInZone)
        {
            return true;
        }
        else return false;
    }
    public void EnableZone()
    {
        triggerZone.enabled = true;
        if (uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(false);
    }

    public void DisableZone()
    {
        triggerZone.enabled = false;
        if (uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(false);

        playerInZone = false;
    }
    private void OnDialogueFinishedHandler()
    {
        npcController?.SetTalking(false);
        isTalking = false;

        DisableZone();       
        uiTalkingPrompt?.SetActive(false);

        playerController?.UnlockMovement();
        switch (gameObject.name)
        {
            case "NPC1":
                GameEventsManager.instance.questEvent.FinishQuest("Quest1Info");
                GameEventsManager.instance.questEvent.StartQuest("Quest2Info");
                break;
            case "NPC2":
                break;  
            case "NPC3":
                SceneManager_.Instance.LoadSceneByName("Quiz", lockCursor: false, showCursor: true);

                break;
            case "NPC4":
                
                break;
            case "NPC5":
                break;
            case "NPC6":
                SceneManager_.Instance.LoadSceneByName("Football", lockCursor: true, showCursor: false) ;
                break;

        }
       
    }

}
