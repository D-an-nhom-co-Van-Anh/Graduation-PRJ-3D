using System;
using TMPro;
using UnityEngine;
public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questState;
    [SerializeField] private TextMeshProUGUI questDescription;
    public void SetInfo(int index, string name,string state, string description)
    {
        questName.SetText("Nhiệm vụ " + (index + 1).ToString());
        questState.SetText(state);
        questDescription.SetText(description);
    }
}
