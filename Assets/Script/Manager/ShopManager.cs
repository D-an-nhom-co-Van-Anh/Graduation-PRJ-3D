using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    [SerializeField] private List <Button> items;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < items.Count; i++)
        {
            items[i].onClick.AddListener(() =>
            {
                BuyItem(items[i].GetComponent<ShopItem>());
            });
        }
    }

    public void BuyItem(ShopItem item)
    {
        GameManager_.Instance.GetCurrencyManager().SubtractCash(item.Cost);
        GameManager_.Instance.GetPlayer().AddStamina(item.Value);
    }
}
