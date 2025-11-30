using UnityEngine;

public class TeleportA2 : MonoBehaviour
{
    public Transform teleportTarget;  

    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (teleportTarget != null)
        {
          
            other.transform.position = teleportTarget.position;
            other.transform.rotation = teleportTarget.rotation;

            if (QuestManager.Instance.GetCurrentQuestID()=="Quest2Info"&& CheckOutsidePoint())
            {
                GameEventsManager.instance.questEvent.FinishQuest("Quest2Info");
                GameEventsManager.instance.questEvent.StartQuest("Quest3Info");
            }
        }
        else
        {
            Debug.LogWarning("? Ch?a g�n teleportTarget cho " + gameObject.name);
        }
    }
    public bool CheckOutsidePoint()
    {
        if (teleportTarget.name == "TeleportA2Outside")
        {
            return true;
        }
        else
        {
            
        }
        return false;
    }
}
