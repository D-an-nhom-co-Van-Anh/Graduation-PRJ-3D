using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager_ : Singleton<GameManager_>
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private PlayerMovementController player;
    public QuestEvent questEvent;
    public int level;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovementController>();
            Debug.Log(player);
        }
        level = 0;
    }
    public void Addlevel()
    {
        level++;
    }
    public void LoadSceneByName()
    {
        SceneManager.LoadScene(1,LoadSceneMode.Additive);
    }
    public CurrencyManager GetCurrencyManager()
    {
        return this.currencyManager;
    }
    public QuestManager GetQuestManager()
    {
        return this.questManager;
    }
    public PlayerMovementController GetPlayer()
    {
        return this.player;
    }
}
