using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager_ : Singleton<GameManager_>
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private SceneManager_ sceneManager;
    [SerializeField] private PlayerMovementController player;
    [SerializeField] private RectTransform UIShop;
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
   
    public CurrencyManager GetCurrencyManager()
    {
        return this.currencyManager;
    }
    public QuestManager GetQuestManager()
    {
        return this.questManager;
    }
    public SceneManager_ GetSceneManager()
    {
        return this.sceneManager;
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
    public void EnableUIShop()
    {
        UIShop.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void DisableUIShop()
    {
        UIShop.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (QuestManager.Instance.GetCurrentQuestID() == "Quest4Info")
        {
            GameEventsManager.instance.questEvent.FinishQuest("Quest4Info");
            GameEventsManager.instance.questEvent.StartQuest("Quest5Info");
        }
    }
}
