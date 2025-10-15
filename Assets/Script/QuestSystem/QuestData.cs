using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    private List<QuestStateSave> data;
    public QuestData(List<QuestStateSave> data)
    {
        this.data = data;
    }
    public List<QuestStateSave> GetList()
    {
        return this.data;
    }
}
