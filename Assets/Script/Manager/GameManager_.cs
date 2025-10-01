using UnityEngine;

public class GameManager_ : Singleton<GameManager_>
{
    public QuestEvent questEvent;
    private void Awake()
    {
        questEvent = new QuestEvent();
    }
}
