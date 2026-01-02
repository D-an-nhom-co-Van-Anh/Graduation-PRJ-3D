using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public string npcRootName = "NPC";

    private void Start()
    {
        GameObject root = GameObject.Find(npcRootName);

        if (root == null)
        {
            Debug.LogError($"Không tìm thấy root: {npcRootName}");
            return;
        }

        for (int i = 0; i < root.transform.childCount; i++)
        {
            Transform npcTransform = root.transform.GetChild(i);
            string npcName = npcTransform.name; 

            string indexStr = npcName.Replace("NPC", "");

            string questId = "Quest" + indexStr + "Info";

            if (QuestManager.Instance.CheckQuest(questId, QuestState.FINISHED))
            {
                NpcTriggerZone zone = npcTransform.GetComponent<NpcTriggerZone>();
                if (zone != null)
                {
                    zone.DisableZone();
                    Debug.Log($"Đã tắt {npcName} vì {questId} đã FINISHED");
                }
            }
        }
    }
}