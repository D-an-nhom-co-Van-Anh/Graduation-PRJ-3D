using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private Dictionary<string, Quest> questMap;
    public QuestManager instance { get; private set; }
    private void Awake()
    {
        questMap=CreateQuestMap();
        Debug.Log(questMap.Count);
        if (instance != null)
        {
            Debug.LogError("Found more than one Quest Manager in the scene.");
        }
        instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(AddQuestEvent());
    }
    private void OnDisable()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.questEvent.onStartQuest -= StartQuest;
            GameEventsManager.instance.questEvent.onAdvanceQuest -= AdvanceQuest;
            GameEventsManager.instance.questEvent.onFinishQuest -= FinishQuest;
        }
    }
    private IEnumerator AddQuestEvent()
    {
        yield return new WaitForSeconds(2f);
        GameEventsManager.instance.questEvent.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvent.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvent.onFinishQuest += FinishQuest;
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
    public Dictionary<string,Quest> GetQuestMap()
    {
        return this.questMap;
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
        SaveQuest();
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
            if (questMap.Count != 0)
            {
                List<QuestStateSave> dataSaveList = new List<QuestStateSave>();
                foreach (Quest quest in questMap.Values)
                {
                    QuestStateSave questData = quest.GetQuestData();
                    dataSaveList.Add(questData);
                }
                QuestData data = new QuestData(dataSaveList);
                string json = JsonUtility.ToJson(data, true);
                string path = Application.persistentDataPath + "/quests.json";
                Debug.Log(json);
                System.IO.File.WriteAllText(path, json);
                Debug.Log("Da luu quest vao: " + path);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
    public string GetCurrentQuestID()
    {
        foreach (var kvp in questMap)
        {
            if (kvp.Value.state == QuestState.IN_PROGRESS|| kvp.Value.state == QuestState.CAN_START)
            {
                return kvp.Value.info.id; 
            }
        }
        return null; 
    }

    private void LoadQuest(Dictionary<string,Quest>questMap)
    {
        string path = Application.persistentDataPath + "/quests.json";

        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning("Không có file lưu quest.");
            return;
        }

        string json = System.IO.File.ReadAllText(path);

        QuestData data = JsonUtility.FromJson<QuestData>(json);

        if (data == null)
        {
            Debug.LogError("Không thể parse JSON → QuestData NULL");
            return;
        }

        if (data.GetList() == null)
        {
            Debug.LogError("questList NULL trong QuestData. JSON sai cấu trúc.");
            return;
        }

        foreach (QuestStateSave q in data.GetList())
        {
            if (q == null)
            {
                Debug.LogWarning("QuestStateSave null trong list, bỏ qua");
                continue;
            }

            if (questMap.TryGetValue(q.questId, out Quest quest))
            {
                quest.state = q.state;
                ChangeQuestState(q.questId, q.state);
                Debug.Log($"{quest.info.id} {quest.state}");
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy quest với id: {q.questId}");
            }
        }
    }
}
