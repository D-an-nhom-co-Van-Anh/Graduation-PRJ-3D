using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using TMPro;
public class ShopItem : MonoBehaviour
{
    [SerializeField]private float cost;
    [SerializeField]private float value;
    [SerializeField] private TextMeshProUGUI price;
    public float Cost => cost;
    public float Value => value;
    private void Awake()
    {
        price.SetText(cost.ToString());
    }
    public void ShowSelectedUI()
    {

    }
    public void HideSelectedUI()
    {

    }
}
