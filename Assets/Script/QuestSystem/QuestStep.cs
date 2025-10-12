using TMPro;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    public void InitializedQuestStep(string questId)
    {
        this.questId = questId;
    }
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            GameEventsManager.instance.questEvent.AdvanceQuest(questId);
            Destroy(this.gameObject);
        }
    }
}
