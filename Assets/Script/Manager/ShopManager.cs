using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    [SerializeField] private List <Button> items;
    private ShopItem currentSelectedItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < items.Count; i++)
        {
            items[i].onClick.AddListener(() =>
            {
                SelectItem(items[i].GetComponent<ShopItem>());
            });
        }
    }
    public void SelectItem(ShopItem item)
    {
        if (currentSelectedItem != null)
        {
            currentSelectedItem.HideSelectedUI();
            currentSelectedItem = null;
            currentSelectedItem = item;
            currentSelectedItem.ShowSelectedUI();
        }
    }
    public void BuyItem()
    {
        if (currentSelectedItem != null)
        {
            GameManager_.Instance.GetCurrencyManager().SubtractCash(currentSelectedItem.Cost);
            GameManager_.Instance.GetPlayer().AddStamina(currentSelectedItem.Value);
        }
        else
        {
            ShowCantBuyText();
        }
    }
    public void ShowCantBuyText()
    {

    }
}
