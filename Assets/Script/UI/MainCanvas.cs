using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : UICanvas
{
    [SerializeField] private Button missionButton;
    [SerializeField] private GameObject missionBoard;
    [SerializeField] private List<TextMeshProUGUI> questState;
    private Dictionary<string, Quest> questMap;
    private void Awake()
    {
        missionButton.onClick.AddListener(() => {

            missionBoard.SetActive(true);
            UpdateMissionBoard();
        });
    }
    public void UpdateMissionBoard()
    {
        questMap= GameManager_.Instance.GetQuestManager().GetQuestMap();
        int i = 0;
        foreach(Quest q in questMap.Values)
        {
            questState[i].SetText(q.state.ToString());
            i++;
        }
    }
}
