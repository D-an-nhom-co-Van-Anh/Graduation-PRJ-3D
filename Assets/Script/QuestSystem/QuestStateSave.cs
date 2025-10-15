using UnityEngine;

[System.Serializable]
public class QuestStateSave 
{
    public string questId;
    public QuestState state;
    public QuestStateSave(string questId, QuestState state)
    {
        this.questId = questId;
        this.state = state;
    }
}
