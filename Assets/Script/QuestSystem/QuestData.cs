    using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class QuestData 
{
    public List<QuestStateSave> data = new List<QuestStateSave>();
    public QuestData(List<QuestStateSave> data)
    {
        this.data = data;
    }
    public List<QuestStateSave> GetList()
    {
        return this.data;
    }
}
