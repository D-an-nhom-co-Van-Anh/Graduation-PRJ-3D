using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameManager_ : Singleton<GameManager_>
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private SceneManager_ sceneManager;
    [SerializeField] private PlayerMovementController player;
    [SerializeField] private RectTransform UIShop;
    [SerializeField] private GameObject mainCanvas;
    
    public PlayableDirector cutSceneinA2;
    private bool isGameStart;
    public QuestEvent questEvent;
    public int level;
    private string saveKey = "Played_Intro_A2";
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
        AudioManager.Instance.PlayMusic("background1");
    }
    public void Addlevel()
    {
        level++;
    }
   
    public void PlayTimeline()
    {
        if (cutSceneinA2 == null) return;
        int hasPlayed = PlayerPrefs.GetInt(saveKey, 0);
        if (hasPlayed == 0)
        {
            cutSceneinA2.enabled = true; 
            cutSceneinA2.Play();
            
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();
        }
    }

    public void SkipTimeLine()
    {
        if(!cutSceneinA2.isActiveAndEnabled) return;
        cutSceneinA2.Stop();
        cutSceneinA2.enabled = false;
        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();
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
        UIShop.GetComponent<ShopManager>().OpenShop();
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
    public GameObject GetMainCanvas()
    {
        return this.mainCanvas;
    }

    public void OnApplicationQuit()
    {
        GetPlayer().SavePlayerData();
    }
}
