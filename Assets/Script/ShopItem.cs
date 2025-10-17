using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField]private float cost;
    [SerializeField]private float value;
    public float Cost => cost;
    public float Value => value;
}
