using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ShopItem : MonoBehaviour
{
    [SerializeField]private float cost;
    [SerializeField]private float value;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private ShopItemType type;
    [SerializeField] private Image selectedUI;
    public float Cost => cost;
    public float Value => value;
    public ShopItemType Type => type;
    private void Awake()
    {
        price.SetText(cost.ToString());
    }
    public void ShowSelectedUI()
    {
       // selectedUI.gameObject.SetActive(true);
    }
    public void HideSelectedUI()
    {
        //selectedUI.gameObject.SetActive(false);
    }
}
public enum ShopItemType
{
    STAMINA =0,
    STAMINA_PERSECOND=5,
    SPEED =10,
}
