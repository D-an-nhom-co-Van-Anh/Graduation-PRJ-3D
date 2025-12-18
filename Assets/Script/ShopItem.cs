using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ShopItem : MonoBehaviour
{
    [SerializeField]private float cost;
    [SerializeField]private float value;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI foodName;
    [SerializeField] private ShopItemType type;
    [SerializeField] private Image selectedUI;
    [SerializeField] private string description;
    public float Cost => cost;
    public float Value => value ;
    public ShopItemType Type => type;
    public string Description => description;
    private void Awake()
    {
        price.SetText(cost.ToString());
        foodName.SetText(this.GetComponent<Image>().sprite.name);
    }

    public void ShowSelectedUI()
    {
       
       selectedUI.gameObject.SetActive(true);
    }
    public void HideSelectedUI()
    {
        if (selectedUI != null)
            selectedUI.gameObject.SetActive(false);
    }
}
public enum ShopItemType
{
    STAMINA =0,
    STAMINA_PERSECOND=5,
    SPEED =10,
}
