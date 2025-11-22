using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : UICanvas
{
    [SerializeField] private Button missionButton;
    [SerializeField] private GameObject missionBoard;
    [SerializeField] private List<QuestUI> questUI;
    private string questState;
    private Dictionary<string, Quest> questMap;
    private void Awake()
    {
        missionButton.onClick.AddListener(() => {

            missionBoard.SetActive(true);
            UpdateMissionBoard();
            Cursor.visible = true;
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
                else
                {
                    questState = q.state.ToString();
                }
                questUI[i].SetInfo(q.info.id.ToString(),questState,q.info.displayName);
            }
            i++;
        }
        if (i < questUI.Count)
        {
            for (int j = i; j < questUI.Count; j++)
            {
                Destroy(questUI[i].gameObject);
            }
        }
                  
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)){
            
            missionBoard.SetActive(true);
            UpdateMissionBoard();
            Cursor.visible = true;
            Cursor.lockState=CursorLockMode.None;
        }
    }
    public void CloseQuestUI()
    {
        missionBoard.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState=CursorLockMode.Locked;
    }
}
