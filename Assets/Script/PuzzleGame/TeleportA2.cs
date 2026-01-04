using System;
using UnityEngine;

public class TeleportA2 : MonoBehaviour
{
    [Header("Target teleport point")]
    public Transform teleportTarget;

    private Rigidbody playerRb;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerRb = playerObj.GetComponent<Rigidbody>();
        }
    }

    public void Teleport()
    {
        if (playerRb == null)
        {
            Debug.LogWarning("Không tìm thấy Player hoặc không có Rigidbody!");
            return;
        }

        if (teleportTarget == null)
        {
            Debug.LogWarning("Chưa gán teleportTarget!");
            return;
        }

        // *** Teleport bằng physics để không bị giật ***
        playerRb.linearVelocity = Vector3.zero;       // reset vận tốc
        playerRb.angularVelocity = Vector3.zero;

        playerRb.MovePosition(teleportTarget.position);
        //playerRb.MoveRotation(teleportTarget.rotation);

        // --- Quest logic ---
        if (QuestManager.Instance.GetCurrentQuestID() == "Quest2Info" && IsOutsidePoint())
        {
            GameEventsManager.instance.questEvent.FinishQuest("Quest2Info");
            GameEventsManager.instance.questEvent.StartQuest("Quest3Info");
        }
    }

    private bool IsOutsidePoint()
    {
        return teleportTarget != null && teleportTarget.name == "TeleportA2Outside";
    }
}