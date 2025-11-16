using System.Collections;
using UnityEngine;

public class QuestPoint : InteractableObj
{
    [SerializeField] private QuestInfoSO questInfoForPoint;
    private bool playerIsNear = false;
    private string questId;
    public QuestState currentQuestState;

    [Header("Config")]
    [SerializeField] private bool isStartPoint;
    [SerializeField] private bool isFinishPoint;
    private void Awake()
    {
        questId = questInfoForPoint.id;
    }
    private void OnEnable()
    {
        StartCoroutine(AddQuestEvent());
    }
    private IEnumerator AddQuestEvent()
    {
        yield return new WaitForSeconds(2f);
        GameEventsManager.instance.questEvent.onQuestStateChange += QuestStateChange;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.questEvent.onQuestStateChange -= QuestStateChange;
    }
    public override void Interact()
    {
        if (!playerIsNear)
        {
            return;
        }
        if (currentQuestState.Equals(QuestState.CAN_START) && isStartPoint)
        {
            GameEventsManager.instance.questEvent.StartQuest(questId);
        }
        else if(currentQuestState.Equals(QuestState.CAN_FINISH) && isFinishPoint)
        {
            GameEventsManager.instance.questEvent.FinishQuest(questId);
        }
    }
    public void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
