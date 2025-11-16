using UnityEngine;
using TMPro;
public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questState;
    [SerializeField] private TextMeshProUGUI questDescription;
    public void SetInfo(string name,string state, string description)
    {
        questName.SetText(name);
        questState.SetText(state);
        //questDescription.SetText(description);
    }
}
