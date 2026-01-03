using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvas : UICanvas
{
    [SerializeField] private Button missionButton;
    [SerializeField] private GameObject missionBoard;
    [SerializeField] private TextMeshProUGUI currentMission;
    [SerializeField] private List<QuestUI> questUI;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollow;
    private string questState;
    private Dictionary<string, Quest> questMap;
    private bool isQuestOpen;
    private int index;
    private void Awake()
    {
        isQuestOpen = false;
        orbitalFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        missionButton.onClick.AddListener(() => {

            OpenQuestUI();
        });
    }
    public void UpdateMissionBoard()
    {
        questMap= GameManager_.Instance.GetQuestManager().GetQuestMap();
        int i = 0;
        foreach(Quest q in questMap.Values)
        {
            if (i < questUI.Count)
            {
                if (q.state == QuestState.REQUIREMENTS_NOT_MET)
                {
                    questState = "        X ";
                }
                else if (q.state == QuestState.IN_PROGRESS)
                {
                    questState = "ON GOING";
                }
                else
                {
                    questState = q.state.ToString();
                }
                questUI[i].SetInfo(i,q.info.id.ToString(),questState,q.info.displayName);
            }
            i++;
        }
                  
    }
    private void Update()
    {
        index = 0;
        questMap = GameManager_.Instance.GetQuestManager().GetQuestMap();
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS|| quest.state == QuestState.CAN_START)
            {
                currentMission.SetText("Nhiệm vụ " + (index + 1).ToString());
                break;
            }
            index++;
        }
        if (Input.GetKeyDown(KeyCode.M)&&!isQuestOpen){
            if (SceneManager.sceneCount > 1)
            {
                return;
            }
            OpenQuestUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape)&&!isQuestOpen)
        {
           OpenAndCloseSettingUI();
        }

    }

    public void OpenAndCloseSettingUI()
    {
        if (UIManager_.Instance.IsOpened<SettingUI>())
        {
            orbitalFollow.enabled =true;
            UIManager_.Instance.CloseDirect<SettingUI>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            orbitalFollow.enabled = false;
            UIManager_.Instance.Open<SettingUI>();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void OpenQuestUI()
    {
        if (orbitalFollow != null)
        {
            Debug.Log("Set false");
            orbitalFollow.enabled = false;
        }
        orbitalFollow.enabled = false;
        missionBoard.SetActive(true);
        UpdateMissionBoard();
        isQuestOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void CloseQuestUI()
    {
        if (orbitalFollow != null)
        {
            orbitalFollow.enabled = true;
        }
        missionBoard.SetActive(false);
        isQuestOpen = false;
        Cursor.visible = false;
        Cursor.lockState=CursorLockMode.Locked;
    }
}
