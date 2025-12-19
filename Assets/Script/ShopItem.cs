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
    private string foodNameText;
    private string descriptionText;
    private void Awake()
    {
        price.SetText(cost.ToString());
        foodNameText = this.GetComponent<Image>().sprite.name;
        foodName.SetText(this.GetComponent<Image>().sprite.name);
        switch (type)
        {
            case ShopItemType.STAMINA:
                descriptionText = "Hồi " + value.ToString() + " stamina";
                break;
            case ShopItemType.STAMINA_PERSECOND:
                descriptionText = "Hồi " + value.ToString() + " stamina mỗi giây";
                break;
            case ShopItemType.SPEED:
                descriptionText = "Tăng thêm " + value.ToString()+ " tốc độ di chuyển";
                break;

        }
        description= "• Món ăn: "+foodNameText+ "\n" +
        "• Giá:" + cost.ToString()+"\n" +
        "• "+descriptionText;
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
