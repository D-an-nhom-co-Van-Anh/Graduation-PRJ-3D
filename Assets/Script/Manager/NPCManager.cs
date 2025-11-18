using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
  

    public List<NpcTriggerZone> npcZones = new List<NpcTriggerZone>();
    public string npcRootName = "NPC";

    private void Awake()
    {
       
        AutoLoadNPCZones();
    }

    private void AutoLoadNPCZones()
    {
        npcZones.Clear();

        GameObject root = GameObject.Find(npcRootName);
        if (root == null)
        {
            Debug.LogError("? Không tìm th?y GameObject cha: " + npcRootName);
            return;
        }

        NpcTriggerZone[] zones = root.GetComponentsInChildren<NpcTriggerZone>(true);
        foreach (var zone in zones)
        {
            npcZones.Add(zone);
        }
    }

   
}
