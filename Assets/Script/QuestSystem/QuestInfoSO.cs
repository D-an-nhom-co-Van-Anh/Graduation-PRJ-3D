using UnityEngine;

[CreateAssetMenu(fileName ="QuestInfoSO",menuName ="ScriptableObject/QuestInfoSO",order =1)]
public class QuestInfoSO : ScriptableObject
{
    [field:SerializeField]public string id { get; private set; }
    [Header("General")]
    public string displayName;
    [Header("Requirements")]
    public QuestInfoSO[] questPrerequisites;
    [Header("Step")]
    public GameObject[] questStepPrefabs;
    [Header("Reward")]
    public float rewardMoney;
    public GameObject rewardObject;
    // dam bao id la ten cua SO 
    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
