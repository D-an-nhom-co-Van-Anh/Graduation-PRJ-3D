using UnityEngine;

public class QuestPoint : InteractableObj
{
    [SerializeField] private QuestInfoSO questInfoForPoint;
    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    [Header("Config")]
    [SerializeField] private bool isStartPoint;
    [SerializeField] private bool isFinishPoint;
    private void Awake()
    {
        questId = questInfoForPoint.id;
    }
    private void OnEnable()
    {
        GameManager_.Instance.questEvent.onQuestStateChange += QuestStateChange;
    }
    private void OnDisable()
    {
        GameManager_.Instance.questEvent.onQuestStateChange -= QuestStateChange;
    }
    public override void Interact()
    {
        if (!playerIsNear)
        {
            return;
        }
        if (currentQuestState.Equals(QuestState.CAN_START) && isStartPoint)
        {
            GameManager_.Instance.questEvent.StartQuest(questId);
        }
        else if(currentQuestState.Equals(QuestState.CAN_FINISH) && isFinishPoint)
        {
            GameManager_.Instance.questEvent.FinishQuest(questId);
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
