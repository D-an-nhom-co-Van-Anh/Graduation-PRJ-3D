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

    private bool playerInZone = false;
    private bool isTalking = false;

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
        // Khi nhấn E trong vùng
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            isTalking = true;
            npcController?.SetTalking(true);

            if (uiTalkingPrompt != null)
                uiTalkingPrompt.SetActive(false);

            // 👉 Xoay NPC về phía player một lần duy nhất
            if (npcTransform != null && playerTransform != null)
            {
                Vector3 direction = (playerTransform.position - npcTransform.position);
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    npcTransform.rotation = Quaternion.Slerp(
                        npcTransform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime * 10f
                    );
                }
            }
        }
    }
}
