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
    [SerializeField]
    private DialogueController dialogueController;


    private void Reset()
    {
        triggerZone = GetComponent<Collider>();
        if (triggerZone != null)
            triggerZone.isTrigger = true;
    }

    private void Start()
    {
        // Tắt UI khi bắt đầu
        if (uiTalkingPrompt != null)
            uiTalkingPrompt.SetActive(false);

        // Nếu chưa gán npcTransform, lấy từ npcController
        if (npcTransform == null && npcController != null)
            npcTransform = npcController.transform;

        // 🔍 Tự tìm Player trong scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("[NpcTriggerZone] Không tìm thấy Player với tag 'Player'!");
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
        // 🔁 Nếu Player spawn muộn => tự tìm lại
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogueController.IsDialogueActive)
            {
                dialogueController.StartDialogue();
                npcController?.SetTalking(true);
            }
            else
            {
                dialogueController.NextDialogue();
                if (!dialogueController.IsDialogueActive)
                {
                    npcController?.SetTalking(false);
                }
            }

            if (uiTalkingPrompt != null)
                uiTalkingPrompt.SetActive(false);
        }

        if (isTalking && playerInZone && playerTransform != null && npcTransform != null)
        {
            if (Time.time >= nextRotationTime)
            {
                RotateTowardPlayer();
                nextRotationTime = Time.time + rotationDelay; 
            }
        }
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
}
