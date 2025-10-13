using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;
    private void Awake()
    {
        questMap=CreateQuestMap();
        Debug.Log(questMap.Count);
    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvent.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvent.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvent.onFinishQuest += FinishQuest;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.questEvent.onStartQuest -= StartQuest;
        GameEventsManager.instance.questEvent.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvent.onFinishQuest -= FinishQuest;
    }
    private void Start()
    {
        foreach (Quest quest in questMap.Values)
        {
            // initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // broadcast the initial state of all quests on startup
            GameEventsManager.instance.questEvent.QuestStateChange(quest);
        }
    }
    private void Update()
    {
        // loop through ALL quests
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }
    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvent.QuestStateChange(quest);
    }
    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;
        // check quest prerequisites for completion
        if (quest.info.questPrerequisites.Count() == 0)
        {
            return meetsRequirements;
        }
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    public void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }
    public void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }
    public void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        Debug.Log(quest.state);
    }
    private void OnApplicationQuit()
    {
        //SaveQuest();
    }
    private Dictionary<string,Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("QuestInfoSO");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach(QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.Log("");
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        LoadQuest(idToQuestMap);
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
    private void ClaimRewards(Quest quest)
    {
        // GameManager_.Instance.GetCurrencyManager().AddCash(quest.info.rewardMoney);
        Debug.Log("Win");
    }
    private void SaveQuest()
    {
        try
        {
            List<QuestStateSave> dataSaveList = new List<QuestStateSave>();
            foreach(Quest quest in questMap.Values)
            {
                QuestStateSave questData = quest.GetQuestData();
                dataSaveList.Add(questData);
            }
            QuestData data = new QuestData(dataSaveList);
            string json = JsonUtility.ToJson(data, true);
            string path = Application.persistentDataPath + "/quests.json";
            System.IO.File.WriteAllText(path, json);
            Debug.Log("Da luu quest vao: " + path);

        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
    private void LoadQuest(Dictionary<string,Quest>questMap)
    {
        string path = Application.persistentDataPath + "/quests.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning("Khong co file luu quest.");
        }
        else
        {
            string json = System.IO.File.ReadAllText(path);
            QuestData data = JsonUtility.FromJson<QuestData>(json);
            foreach(QuestStateSave q in data.GetList())
            {
                if (questMap.TryGetValue(q.questId, out Quest quest))
                {
                    quest.state = q.state;
                }
            }
        }
    }
}
