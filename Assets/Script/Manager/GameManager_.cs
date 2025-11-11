using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager_ : Singleton<GameManager_>
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private PlayerMovementController player;
    private bool isGameStart;
    public QuestEvent questEvent;
    public int level;
    public bool IsGameStart=>isGameStart;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovementController>();
            Debug.Log(player);
        }
        isGameStart = false;
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
        if (this.player == null)
        {
            this.player = FindObjectOfType<PlayerMovementController>();
        }
        return this.player;
    }
    public void StartGame()
    {
        isGameStart = true;
    }
}
