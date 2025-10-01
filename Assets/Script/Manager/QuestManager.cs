using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;
    private void Awake()
    {
        questMap=CreateQuestMap();
    }

    private void OnEnable()
    {
        GameManager_.Instance.questEvent.onStartQuest += StartQuest;
        GameManager_.Instance.questEvent.onAdvanceQuest += AdvanceQuest;
        GameManager_.Instance.questEvent.onFinishQuest += FinishQuest;
    }
    private void OnDisable()
    {
        GameManager_.Instance.questEvent.onStartQuest -= StartQuest;
        GameManager_.Instance.questEvent.onAdvanceQuest -= AdvanceQuest;
        GameManager_.Instance.questEvent.onFinishQuest -= FinishQuest;
    }
    private void Start()
    {
        foreach (Quest quest in questMap.Values)
        {
            GameManager_.Instance.questEvent.QuestStateChange(quest);
        }
    }
    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameManager_.Instance.questEvent.QuestStateChange(quest);
    }
    public void StartQuest(string id)
    {

    }
    public void AdvanceQuest(string id)
    {

    }
    public void FinishQuest(string id)
    {

    }
    private Dictionary<string,Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach(QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.Log("");
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }
    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.Log("Err");
        }
        return quest;
    }
}
