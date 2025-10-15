using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager_ : Singleton<GameManager_>
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private CurrencyManager currencyManager;
    public QuestEvent questEvent;
    public int level;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        level = 0;
    }
    public void Addlevel()
    {
        level++;
    }
    public void LoadSceneByName()
    {
        SceneManager.LoadScene(1);
    }
    public CurrencyManager GetCurrencyManager()
    {
        return this.currencyManager;
    }
    public QuestManager GeTQuestManager()
    {
        return this.questManager;
    }
}
